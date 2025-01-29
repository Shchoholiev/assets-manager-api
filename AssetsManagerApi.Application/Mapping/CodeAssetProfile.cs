using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class CodeAssetProfile : Profile
{
    public CodeAssetProfile()
    {
        CreateMap<CodeAsset, CodeAssetDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.LanguageToString(src.Language)));
        CreateMap<CodeAssetDto, CodeAsset>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.StringToLanguage(src.Language)));
    }
}
