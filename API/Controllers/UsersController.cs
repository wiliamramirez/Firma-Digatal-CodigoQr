using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public UsersController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper= mapper;
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

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Dni))
            {
                return BadRequest("Ya existe un usuario con este Dni");
            }

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();

            user.Id = Guid.NewGuid();
            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            var rolesDto = new List<RoleDto>();

            foreach (var role in registerDto.Roles)
            {
                var userRole = new UserRole
                {
                    RoleId = role.Id,
                    AppUser = user
                };

                _context.UserRoles.Add(userRole);

                var rol = await _context.Roles.FirstOrDefaultAsync(x => x.Id == userRole.RoleId);
                var rolDto = new RoleDto
                {
                    Name = rol.Name
                };
                rolesDto.Add(rolDto);
            }


            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    LastName = user.LastName,
                    Position = user.Position,
                    Token = _tokenService.CreateToken(user),
                    Username = user.UserName,
                    Dni = user.Dni,
                    Roles = rolesDto
                };
            }


            return BadRequest("No se pudo registrar el usuario");
        }

        private async Task<bool> UserExists(string dni)
        {
            return await _context.Users.AnyAsync(x => x.Dni == dni);
        }

    }
}