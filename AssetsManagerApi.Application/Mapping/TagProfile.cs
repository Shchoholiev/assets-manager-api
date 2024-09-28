using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class TagProfile : Profile
{
    public TagProfile()
    {
        CreateMap<TagDto, Tag>().ReverseMap();
    }
}
