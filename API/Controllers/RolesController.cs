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
    [AllowAnonymous]
    public class RolesController: BaseApiController
    {
        private readonly DataContext _context;


        public RolesController(DataContext context)
        {
            _context = context;

        }

        [HttpGet]

        public async Task<ActionResult<List<RoleDto>>> GetRole()

        {
            var rolesDto = new List<RoleDto>();
            var roles = await _context.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var roleDto = new RoleDto
                {
                    Name = role.Name
                };
                
                rolesDto.Add(roleDto);
            }

            return rolesDto;
            
        }

        [HttpPost("add")]
        public async Task<ActionResult<RoleDto>> AddRole(RoleDto roleDto)
        {
            if (roleDto==null)
            {
                return BadRequest();
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleDto.Name,
            };

            _context.Roles.Add(role);
            var resultContex = await _context.SaveChangesAsync();

            var resultRoleDto = new RoleDto
            {
                Name = roleDto.Name
            };

            if (resultContex>0)
            {
                return Ok(resultRoleDto);
            }

            return BadRequest();
        }
    }
}