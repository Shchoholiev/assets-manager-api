using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;

namespace AssetsManagerApi.Application.Mapping;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<CompanyDto, Company>().ReverseMap();
    }
}
