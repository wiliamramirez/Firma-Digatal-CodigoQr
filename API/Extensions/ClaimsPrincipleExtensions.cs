using System;
using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetDni(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }

        public static Guid GetId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.Name)?.Value;
            return Guid.Parse(id);
        }

        public static string GetSurname(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Surname)?.Value;
        }
    }
}