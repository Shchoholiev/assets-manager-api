namespace AssetsManagerApi.Api.Models;

/// <summary>
/// Request model for assigning a role to a user.
/// </summary>
public class RoleRequestModel
{
    /// <summary>
    /// Name of the role to assign.
    /// </summary>
    public string RoleName { get; set; }
}