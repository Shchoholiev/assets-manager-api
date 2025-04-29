using System.Collections.Generic;

namespace AssetsManagerApi.Application.Models.Dto;

public class CompanyDto
{
    public string Id { get; set; }

    public string Description { get; set; }

    public string Name { get; set; }

    /// <summary>
    /// The users that belong to the company.
    /// </summary>
    public List<UserDto> Users { get; set; } = new List<UserDto>();
}
