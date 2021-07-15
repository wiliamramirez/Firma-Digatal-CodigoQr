using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers

{
    public class RolesController : BaseApiController
    {
        private readonly DataContext _context;


        public RolesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<RoleDto>>> List()

        {
            var rolesDto = new List<RoleDto>();
            var roles = await _context.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name
                };

                rolesDto.Add(roleDto);
            }

            return rolesDto;
        }

        [HttpPost("add")]
        public async Task<ActionResult<RoleDto>> AddRole(AddRoleDto addRoleDto)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = addRoleDto.Name,
            };

            _context.Roles.Add(role);
            var resultContext = await _context.SaveChangesAsync();

            var resultRoleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name
            };

            if (resultContext > 0)
            {
                return Ok(resultRoleDto);
            }

            return BadRequest();
        }
    }
}