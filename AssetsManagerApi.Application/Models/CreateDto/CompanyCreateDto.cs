namespace AssetsManagerApi.Application.Models.CreateDto;

/// <summary>
/// DTO for creating a new company.
/// </summary>
public class CompanyCreateDto
{
    /// <summary>
    /// The name of the company.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// A brief description of the company.
    /// </summary>
    public string Description { get; set; }
}