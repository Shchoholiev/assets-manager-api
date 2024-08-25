using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities.Identity;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>();
        CreateMap<RoleDto, Role>();
    }
}