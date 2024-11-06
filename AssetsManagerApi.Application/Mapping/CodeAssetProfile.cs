using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class CodeAssetProfile : Profile
{
    public CodeAssetProfile()
    {
        CreateMap<CodeAssetDto, CodeAsset>().ReverseMap();
    }
}
