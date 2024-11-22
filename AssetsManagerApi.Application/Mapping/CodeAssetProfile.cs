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
        CreateMap<CodeAssetDto, CodeAsset>().ReverseMap();

        CreateMap<CodeAssetDto, CodeAssetResult>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.LanguageToString(src.Language)));
        CreateMap<CodeAssetResult, CodeAssetDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.StringToLanguage(src.Language)));

        CreateMap<CodeAsset, CodeAssetResult>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.LanguageToString(src.Language)));
        CreateMap<CodeAssetResult, CodeAsset>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.StringToLanguage(src.Language)));
    }
}
