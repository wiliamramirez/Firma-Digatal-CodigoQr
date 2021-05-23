using Microsoft.AspNetCore.Http;

namespace API.DTOs
{
    public class AddDocumentDto
    {
        public string Affair { get; set; }
        public string Title { get; set; }
        public IFormFile File { get; set; }
    }
}