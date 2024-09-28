using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class FileSystemNodeProfile : Profile
{
    public FileSystemNodeProfile()
    {
        CreateMap<FileSystemNodeDto, FileSystemNode>().ReverseMap();
    }
}
