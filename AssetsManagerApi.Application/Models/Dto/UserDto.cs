﻿namespace AssetsManagerApi.Application.Models.Dto;

public class UserDto
{
    public string Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? CompanyId { get; set; }

    public List<RoleDto> Roles { get; set; }
}
