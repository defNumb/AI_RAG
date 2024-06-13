using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_RAG.Models;

namespace AI_RAG.Interfaces
{
    public interface IVectorStore
    {
        Task AddVectorAsync(Vector vector);
        Task UpdateVectorAsync(Vector vector);
        Task DeleteVectorAsync(string vectorId);
        Task<Vector> GetVectorAsync(string vectorId);
        Task<IEnumerable<QueryResult>> QueryVectorsAsync(float[] queryVector, int topK);
    }

    public interface IRetrievalService
    {
        Task<IEnumerable<Document>> RetrieveDocumentsAsync(string query);
    }

    public interface IAugmentationService
    {
        Task<string> AugmentResponseAsync(string query, IEnumerable<Document> documents);
    }
}
