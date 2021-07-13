using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous]
    //BaseApiController tiene la base de la url api/[controller] en controller dentro del corchete significa que va tomar el nombre del controlador
    public class AccountsController : BaseApiController
    {
        private readonly DataContext _context; //instancia a la orm 
        private readonly IMapper _mapper; //permite instanciar clases sin utilizar otras

        private readonly ITokenService _tokenService; //instancia del servicio del toquen 
        //se instancia para guradar toquen pero no para ver si existe 

        public AccountsController(DataContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(y => y.Role)
                .SingleOrDefaultAsync(x => x.Dni == loginDto.Dni);
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

            var rolesDto = new List<RoleDto>();

            foreach (var role in user.UserRoles)
            {
                var rolDto = new RoleDto
                {
                    Name = role.Role.Name
                };

                rolesDto.Add(rolDto);
            }

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


        private async Task<bool> UserExists(string dni)
        {
            return await _context.Users.AnyAsync(x => x.Dni == dni);
        }
    }
}