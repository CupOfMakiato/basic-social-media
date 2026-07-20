using AutoMapper;
using BasicSocialMedia.Application.DTOs.Message;
using BasicSocialMedia.Application.DTOs.User;
using BasicSocialMedia.Domain.Entities;

namespace BasicSocialMedia.Application.Mappers
{
    public class MapperConfigurationsProfile : Profile
    {
        public MapperConfigurationsProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(
                    destination => destination.ProfilePictureUrl,
                    options => options.MapFrom(source => source.ProfilePicture == null
                        ? null
                        : source.ProfilePicture.FileUrl));

            CreateMap<User, ViewUser>()
                .ForMember(
                    destination => destination.ProfilePictureUrl,
                    options => options.MapFrom(source => source.ProfilePicture == null
                        ? null
                        : source.ProfilePicture.FileUrl));

            CreateMap<Message, MessageDTO>()
                .ForMember(
                    destination => destination.MediaUrls,
                    options => options.MapFrom(source => source.Media
                        .Where(media => !media.IsDeleted)
                        .Select(media => media.FileUrl)
                        .ToList()));
        }
    }
}
