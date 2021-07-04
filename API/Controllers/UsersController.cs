using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;

        public UsersController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var dni = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Dni == dni);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                LastName = user.LastName,
                Position = user.Position,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName,
                Dni = user.Dni
            };
        }
    }
}