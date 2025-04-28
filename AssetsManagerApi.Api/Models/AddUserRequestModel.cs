namespace AssetsManagerApi.Api.Models;

/// <summary>
/// Request model for adding a user to a company by email.
/// </summary>
public class AddUserRequestModel
{
    /// <summary>
    /// Email of the user to add to the company.
    /// </summary>
    public string Email { get; set; }
}