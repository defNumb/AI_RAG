using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI_RAG.Interfaces;
using AI_RAG.Models;
using AI_RAG.Services;
using Microsoft.Extensions.Logging;

namespace AI_RAG
{
    public class RetrievalService : IRetrievalService
    {
        private readonly IVectorStore _vectorStore;
        private readonly EmbeddingsService _embeddingsService;
        private readonly ILogger<RetrievalService> _logger;

        public RetrievalService(IVectorStore vectorStore, EmbeddingsService embeddingsService, ILogger<RetrievalService> logger)
        {
            _vectorStore = vectorStore;
            _embeddingsService = embeddingsService;
            _logger = logger;
        }

        public async Task<IEnumerable<Document>> RetrieveDocumentsAsync(string query)
        {
            // Generate embedding for the query
            float[] queryEmbedding = await _embeddingsService.GetEmbeddingAsync(query);

            // Query the vector store to get relevant document vectors
            var queryResults = await _vectorStore.QueryVectorsAsync(queryEmbedding, 1);

            // List to hold the retrieved documents
            var documents = queryResults.Select(result => result.Document).ToList();

            return documents;
        }
    }
}
