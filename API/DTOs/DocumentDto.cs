using System;

namespace API.DTOs
{
    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Affair { get; set; }
        public string Title { get; set; }
        public string Hash { get; set; }
    }
}