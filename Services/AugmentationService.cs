using AI_RAG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_RAG.Models;
using OpenAI_API;

namespace AI_RAG.Services
{
    public class AugmentationService : IAugmentationService
    {
        private readonly OpenAIAPI _openAIAPI;

        public AugmentationService(string apiKey)
        {
            _openAIAPI = new OpenAIAPI(apiKey);
        }

        public async Task<string> AugmentResponseAsync(string query, IEnumerable<Document> documents)
        {
            // Combine the query and the content of the documents
            var context = GenerateContext(query, documents);

            // Call OpenAI API to get the response
            var result = await _openAIAPI.Completions.CreateCompletionAsync(new OpenAI_API.Completions.CompletionRequest
            {
                Prompt = context,
                MaxTokens = 150
            });

            return result.Completions[0].Text.Trim();
        }

        private string GenerateContext(string query, IEnumerable<Document> documents)
        {
            var context = query + "\n\n";
            foreach (var doc in documents)
            {
                context += doc.Content + "\n\n";
            }
            return context;
        }
    }
}
