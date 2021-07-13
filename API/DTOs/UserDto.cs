
using System;
using System.Collections.Generic;


namespace API.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Dni { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}