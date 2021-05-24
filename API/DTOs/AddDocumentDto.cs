using API.Validation;
using Microsoft.AspNetCore.Http;

namespace API.DTOs
{
    public class AddDocumentDto
    {
        public string Affair { get; set; }
        public string Title { get; set; }

        [FileSizeValidation(30)]
        [FileTypeValidation(FileTypeGroup.File)]
        public IFormFile File { get; set; }
    }
}