using AI_RAG.Interfaces;
using AI_RAG.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AI_RAG.Services
{
    public class PineconeService : IVectorStore
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ILogger<PineconeService> _logger;

        public PineconeService(string apiKey, string baseUrl, ILogger<PineconeService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _baseUrl = baseUrl;
            _httpClient.DefaultRequestHeaders.Add("Api-Key", _apiKey);
            _logger = logger;
        }

        public async Task AddVectorAsync(Vector vector)
        {
            var content = new StringContent(JsonSerializer.Serialize(vector), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/vectors", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateVectorAsync(Vector vector)
        {
            var content = new StringContent(JsonSerializer.Serialize(vector), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/vectors/{vector.Id}", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteVectorAsync(string vectorId)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/vectors/{vectorId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<Vector> GetVectorAsync(string vectorId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vectors/{vectorId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Vector>(content);
        }

        public async Task<IEnumerable<QueryResult>> QueryVectorsAsync(float[] queryVector, int topK)
        {
            var query = new
            {
                vector = queryVector,
                top_k = topK,
                include_metadata = true
            };

            var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/query", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            try
            {
                var pineconeResponse = JsonSerializer.Deserialize<PineconeQueryResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var queryResults = new List<QueryResult>();

                // Filter and sort results based on score
                var filteredMatches = pineconeResponse.Matches
                    .Where(match => match.Metadata != null && match.Metadata.ContainsKey("content"))
                    .OrderByDescending(match => match.Score)
                    .Take(topK); // Adjust topK or use a different threshold if needed

                foreach (var match in filteredMatches)
                {
                    string documentContent = match.Metadata.ContainsKey("content") ? match.Metadata["content"] : string.Empty;

                    queryResults.Add(new QueryResult
                    {
                        Id = match.Id,
                        Score = match.Score,
                        Document = new Document
                        {
                            Id = match.Id,
                            Content = documentContent,
                            Metadata = match.Metadata
                        }
                    });

                    if (string.IsNullOrEmpty(documentContent))
                    {
                        _logger.LogWarning($"No content found in metadata for document ID: {match.Id}");
                    }
                    else
                    {
                        _logger.LogInformation($"Retrieved content for document ID: {match.Id}");
                    }
                }

                return queryResults;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Failed to deserialize JSON: {responseContent}");
                throw new Exception("Error deserializing the response from Pinecone.", ex);
            }
        }

    }
}
