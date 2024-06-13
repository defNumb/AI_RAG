using System.Collections.Generic;

namespace AI_RAG.Models
{
    public class Document
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class Vector
    {
        public string Id { get; set; }
        public float[] Values { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class QueryResult
    {
        public string Id { get; set; }
        public float Score { get; set; }
        public Document Document { get; set; }
    }

    public class PineconeQueryResponse
    {
        public List<PineconeMatch> Matches { get; set; }
    }

    public class PineconeMatch
    {
        public string Id { get; set; }
        public float Score { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}