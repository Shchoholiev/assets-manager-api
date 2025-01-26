namespace AssetsManagerApi.Application.Models.Global;

public static class GlobalUser
{
    public static string? Id { get; set; }

    public static string? Name { get; set; }

    public static string? Email { get; set; }

    public static string? CompanyId { get; set; }

    public static List<string> Roles { get; set; } = [];
}
