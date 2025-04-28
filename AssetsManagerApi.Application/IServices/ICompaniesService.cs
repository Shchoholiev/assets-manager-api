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
    
    /// <summary>
    /// Adds a user to the current user's company by email.
    /// </summary>
    /// <param name="email">The email of the user to add.</param>
    Task<UserDto> AddUserToCompanyAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from the current user's company.
    /// </summary>
    /// <param name="userId">The ID of the user to remove.</param>
    Task<UserDto> RemoveUserFromCompanyAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Assigns a role to a user within the current user's company. For now, only 'Admin' is supported.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="roleName">The name of the role to assign (only 'Admin' supported).</param>
    Task<UserDto> AssignCompanyUserRoleAsync(string userId, string roleName, CancellationToken cancellationToken);
}

