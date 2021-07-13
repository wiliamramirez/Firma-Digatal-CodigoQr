using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class UserRole
    {
        public Guid AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Guid RoleId { get; set; }
        public Role Roles { get; set; }
        
    }
}   