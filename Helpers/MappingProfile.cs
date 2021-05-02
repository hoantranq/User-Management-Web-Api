using AutoMapper;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Models;

namespace UserManagement_Backend.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserForRegisterDto, User>();
        }
    }
}
