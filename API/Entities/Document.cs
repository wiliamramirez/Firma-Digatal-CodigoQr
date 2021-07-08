using System;

namespace API.Entities
{
    public class Document
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Affair { get; set; }
        public string Title { get; set; }
        public string HashSecret { get; set; }
        
        public string User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /*Relacion con la tabla users*/
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}