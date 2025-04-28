using AssetsManagerApi.Application.Exceptions;
using AssetsManagerApi.Application.IRepositories;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Global;
using AssetsManagerApi.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.Infrastructure.Services;

/// <summary>
/// Initializes a new instance of <see cref="CompaniesService"/>.
/// </summary>
public class CompaniesService(
    ICompaniesRepository companiesRepository,
    IUsersRepository usersRepository,
    IRolesRepository rolesRepository,
    IMapper mapper,
    ILogger<CompaniesService> logger) : ICompaniesService
{
    private readonly IMapper _mapper = mapper;
    private readonly ICompaniesRepository _companiesRepository = companiesRepository;
    private readonly IUsersRepository _usersRepository = usersRepository;
    private readonly IRolesRepository _rolesRepository = rolesRepository;
    private readonly ILogger<CompaniesService> _logger = logger;

    public async Task<CompanyDto> GetUsersCompanyAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting company for user {UserId}", GlobalUser.Id);
        if (GlobalUser.CompanyId == null)
        {
            _logger.LogWarning("User {UserId} does not have a company", GlobalUser.Id);
            throw new EntityNotFoundException("User does not have company");
        }

        var entity = await this._companiesRepository.GetOneAsync(GlobalUser.CompanyId, cancellationToken);

        if (entity == null)
        {
            _logger.LogWarning("Company {CompanyId} not found for user {UserId}", GlobalUser.CompanyId, GlobalUser.Id);
            throw new EntityNotFoundException("User's company not found");
        }

        _logger.LogInformation("Found company {CompanyId} for user {UserId}", entity.Id, GlobalUser.Id);
        var dto = _mapper.Map<CompanyDto>(entity);

        return dto;
    }

    /// <summary>
    /// Creates a new company and assigns the creating user as a company administrator.
    /// </summary>
    public async Task<CompanyDto> CreateCompanyAsync(CompanyCreateDto createDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating company {CompanyName} for user {UserId}", createDto.Name, GlobalUser.Id);
        // Create and save the company
        var company = new Company
        {
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedById = GlobalUser.Id!,
            CreatedDateUtc = DateTime.UtcNow
        };
        var createdCompany = await _companiesRepository.AddAsync(company, cancellationToken);
        _logger.LogInformation("Company {CompanyId} created successfully for user {UserId}", createdCompany.Id, GlobalUser.Id);

        // Assign 'Enterprise' and 'Admin' roles to the user
        var enterpriseRole = await _rolesRepository.GetOneAsync(r => r.Name == "Enterprise", cancellationToken)
            ?? throw new EntityNotFoundException("Role 'Enterprise' not found.");
        var adminRole = await _rolesRepository.GetOneAsync(r => r.Name == "Admin", cancellationToken)
            ?? throw new EntityNotFoundException("Role 'Admin' not found.");

        var user = await _usersRepository.GetOneAsync(GlobalUser.Id!, cancellationToken)
            ?? throw new EntityNotFoundException("User not found.");
        _logger.LogInformation("Assigning roles {EnterpriseRole} and {AdminRole} to user {UserId} for company {CompanyId}", enterpriseRole.Name, adminRole.Name, GlobalUser.Id, createdCompany.Id);
        user.CompanyId = createdCompany.Id;
        if (!user.Roles.Any(r => r.Name == enterpriseRole.Name))
            user.Roles.Add(enterpriseRole);
        if (!user.Roles.Any(r => r.Name == adminRole.Name))
            user.Roles.Add(adminRole);
        await _usersRepository.UpdateUserAsync(user, cancellationToken);
        _logger.LogInformation("User {UserId} updated with company {CompanyId} and assigned roles", GlobalUser.Id, createdCompany.Id);

        return _mapper.Map<CompanyDto>(createdCompany);
    }
   
    // New methods for managing company users
    public async Task<UserDto> AddUserToCompanyAsync(string email, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin {AdminId} requests to add user with email {EmailToAdd} to company {CompanyId}", GlobalUser.Id, email, GlobalUser.CompanyId);
        if (GlobalUser.CompanyId == null)
        {
            _logger.LogWarning("User {UserId} has no company to add members", GlobalUser.Id);
            throw new EntityNotFoundException("User does not have a company");
        }
        // Find a user by email who is not already in a company
        var user = await _usersRepository.GetOneAsync(u => u.Email == email && u.CompanyId == null, cancellationToken)
            ?? throw new EntityNotFoundException("User not found or already part of a company.");
        var enterpriseRole = await _rolesRepository.GetOneAsync(r => r.Name == "Enterprise", cancellationToken)
            ?? throw new EntityNotFoundException("Role 'Enterprise' not found.");
        user.CompanyId = GlobalUser.CompanyId;
        if (!user.Roles.Any(r => r.Name == enterpriseRole.Name))
            user.Roles.Add(enterpriseRole);
        await _usersRepository.UpdateUserAsync(user, cancellationToken);
        _logger.LogInformation("User with email {EmailToAdd} successfully added to company {CompanyId}", email, user.CompanyId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> RemoveUserFromCompanyAsync(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin {AdminId} requests to remove user {UserIdToRemove} from company {CompanyId}", GlobalUser.Id, userId, GlobalUser.CompanyId);
        if (GlobalUser.CompanyId == null)
        {
            _logger.LogWarning("User {UserId} has no company to remove members", GlobalUser.Id);
            throw new EntityNotFoundException("User does not have a company");
        }
        var user = await _usersRepository.GetOneAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException("User not found.");
        if (user.CompanyId != GlobalUser.CompanyId)
        {
            throw new EntityNotFoundException("User is not part of this company.");
        }
        user.CompanyId = null;
        user.Roles.RemoveAll(r => r.Name == "Enterprise");
        user.Roles.RemoveAll(r => r.Name == "Admin");
        await _usersRepository.UpdateUserAsync(user, cancellationToken);
        _logger.LogInformation("User {UserIdToRemove} removed from company {CompanyId}", userId, GlobalUser.CompanyId);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> AssignCompanyUserRoleAsync(string userId, string roleName, CancellationToken cancellationToken)
    {
        if (!string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Role {RoleName} not supported for company users", roleName);
            throw new ArgumentException($"Role '{roleName}' not supported for company users.");
        }

        _logger.LogInformation("Admin {AdminId} requests to assign role {RoleName} to user {UserId} in company {CompanyId}", GlobalUser.Id, roleName, userId, GlobalUser.CompanyId);

        if (GlobalUser.CompanyId == null)
        {
            _logger.LogWarning("User {UserId} has no company to assign roles", GlobalUser.Id);
            throw new EntityNotFoundException("User does not have a company");
        }
        var user = await _usersRepository.GetOneAsync(userId, cancellationToken)
            ?? throw new EntityNotFoundException("User not found.");
        if (user.CompanyId != GlobalUser.CompanyId)
        {
            throw new EntityNotFoundException("User is not part of this company.");
        }
        var adminRole = await _rolesRepository.GetOneAsync(r => r.Name == "Admin", cancellationToken)
            ?? throw new EntityNotFoundException("Role 'Admin' not found.");
        if (!user.Roles.Any(r => r.Name == adminRole.Name))
            user.Roles.Add(adminRole);
        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} assigned role {RoleName} in company {CompanyId}", userId, roleName, user.CompanyId);

        return _mapper.Map<UserDto>(user);
    }
}
