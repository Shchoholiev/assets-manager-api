using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace AssetsManagerApi.Application.IServices;

/// <summary>
/// Service interface for managing companies.
/// </summary>
public interface ICompaniesService
{
    /// <summary>
    /// Retrieves the company associated with the current user.
    /// </summary>
    Task<CompanyDto> GetUsersCompanyAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Creates a new company and assigns the current user as its administrator.
    /// </summary>
    Task<CompanyDto> CreateCompanyAsync(CompanyCreateDto createDto, CancellationToken cancellationToken);
}

