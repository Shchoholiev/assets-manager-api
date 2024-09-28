using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class CodeFileProfile : Profile
{
    public CodeFileProfile()
    {
        CreateMap<CodeFile, CodeFileDto>().ReverseMap();
    }
}
