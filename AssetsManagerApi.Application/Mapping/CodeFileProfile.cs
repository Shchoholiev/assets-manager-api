using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Enums;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class CodeFileProfile : Profile
{
    public CodeFileProfile()
    {
        CreateMap<CodeFile, CodeFileDto>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.LanguageToString(src.Language)));
        CreateMap<CodeFileDto, CodeFile>()
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => LanguagesExtensions.StringToLanguage(src.Language)));
    }
}
