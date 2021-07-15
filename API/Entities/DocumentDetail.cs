using System;

namespace API.Entities
{
    public class DocumentDetail
    {
        public Guid Id { get; set; }
        public string Affair { get; set; }
        public string Title { get; set; }
        public string HashSecretMD5 { get; set; }
        public string HashSecretSha256 { get; set; }
        public string User { get; set; } //

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid DocumentId { get; set; }

        public Document Document { get; set; }
    }
}