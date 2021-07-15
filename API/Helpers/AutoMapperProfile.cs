using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, RegisterDto>().ReverseMap();
            CreateMap<AppUser, UserDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}