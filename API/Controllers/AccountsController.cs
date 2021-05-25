using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountsController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public AccountsController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new UserDto
                {
                    Email = user.Email,
                    Fullname = user.FullName,
                    Name = user.Name,
                    Position = user.Position,
                    Token = "token",
                    Username = user.UserName,
                    Dni = user.Dni
                };
            }


            return BadRequest("No se pudo registrar el usuario");
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Dni == loginDto.Dni);
            if (user == null)
            {
                return Unauthorized("Dni invalido");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Password incorrecto");
            }

            return new UserDto
            {
                Email = user.Email,
                Fullname = user.FullName,
                Name = user.Name,
                Position = user.Position,
                Token = "token",
                Username = user.UserName,
                Dni = user.Dni
            };
        }

        private async Task<bool> UserExists(string dni)
        {
            return await _context.Users.AnyAsync(x => x.Dni == dni);
        }
    }
}