using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities.Identity;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
    }
}
