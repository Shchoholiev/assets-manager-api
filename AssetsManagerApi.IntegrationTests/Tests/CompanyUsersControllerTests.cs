using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Api.Models;

namespace AssetsManagerApi.IntegrationTests.Tests;

/// <summary>
/// Integration tests for managing users in a company.
/// </summary>
public class CompanyUsersControllerTests(TestingFactory<Program> factory)
    : TestsBase(factory, "companies")
{
    [Fact]
    public async Task AddUserToCompany_AdminUser_AddsUser()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "a5e00e4b-4c5a-4e08-932f-5b579d5c3f8f"; // test user without company
        var request = new AddUserRequestModel { Email = "userToAddToCompany@gmail.com" };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
        Assert.Equal("67a87bdb92156dc8ddd81daa", user.CompanyId);
        Assert.Contains(user.Roles, r => r.Name == "Enterprise");
    }

    [Fact]
    public async Task RemoveUserFromCompany_AdminUser_RemovesUser()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "a5e00e2b-4c5a-3e08-932f-5b579d5c3f8f"; // seeded enterprise user

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Null(user.CompanyId);
        Assert.DoesNotContain(user.Roles, r => r.Name == "Enterprise");
    }

    [Fact]
    public async Task AssignCompanyAdmin_AdminUser_PromotesUser()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "a5e00e4b-4c5a-4e08-932f-5b579d5c3f8f"; // seeded enterprise user
        var request = new RoleRequestModel { RoleName = "Admin" };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/users/{userId}/roles", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
        Assert.Equal("67a87bdb92156dc8ddd81daa", user.CompanyId);
        Assert.Contains(user.Roles, r => r.Name == "Admin");
    }

    [Fact]
    public async Task AddUserToCompany_NonAdmin_Forbidden()
    {
        // Arrange: enterprise user without admin role
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");
        var userId = "652c3b89ae02a3135d6309fc"; // group test user
        var request = new AddUserRequestModel { Email = userId };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/users", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}