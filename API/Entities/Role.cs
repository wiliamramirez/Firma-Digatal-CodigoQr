using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; }
    }
}