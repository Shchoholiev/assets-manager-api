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
}
