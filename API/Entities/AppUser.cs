using System;

namespace API.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}