namespace AssetsManagerApi.Domain.Entities.Identity;

public class User : EntityBase
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? CompanyId { get; set; }

    public List<Role> Roles { get; set; }
}
