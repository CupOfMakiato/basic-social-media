using AutoMapper;
using BasicSocialMedia.Application.DTOs.User;
using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
