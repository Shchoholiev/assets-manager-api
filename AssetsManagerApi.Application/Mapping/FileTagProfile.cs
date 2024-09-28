using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class FileTagProfile : Profile
{
    public FileTagProfile()
    {
        CreateMap<FileTag, FileTagDto>().ReverseMap();
    }
}
