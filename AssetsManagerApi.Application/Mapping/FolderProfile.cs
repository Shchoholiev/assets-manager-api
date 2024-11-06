using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class FolderProfile : Profile
{
    public FolderProfile()
    {
        CreateMap<Folder, FolderDto>().ReverseMap();
    }
}
