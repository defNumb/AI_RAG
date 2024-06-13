using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_RAG.Services
{
    public class EmbeddingsService
    {
        private readonly OpenAIAPI _openAIAPI;

        public EmbeddingsService(string apiKey)
        {
            _openAIAPI = new OpenAIAPI(apiKey);
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var result = await _openAIAPI.Embeddings.CreateEmbeddingAsync(text);
            return result.Data[0].Embedding;
        }

        public async Task<IEnumerable<float[]>> GetEmbeddingsAsync(IEnumerable<string> texts)
        {
            var embeddings = new List<float[]>();
            foreach (var text in texts)
            {
                embeddings.Add(await GetEmbeddingAsync(text));
            }
            return embeddings;
        }
    }
}
