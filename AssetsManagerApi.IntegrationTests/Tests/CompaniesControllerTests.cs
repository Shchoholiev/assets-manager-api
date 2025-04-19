using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;

namespace AssetsManagerApi.IntegrationTests.Tests;

/// <summary>
/// Integration tests for the CompaniesController.
/// </summary>
public class CompaniesControllerTests(TestingFactory<Program> factory) 
    : TestsBase(factory, "companies")
{
    [Fact]
    public async Task CreateCompany_Authenticated_ReturnsCreated()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var createDto = new CompanyCreateDto
        {
            Name = "NewCo",
            Description = "A newly created company."
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", createDto);

        var str = await response.Content.ReadAsStringAsync();
        Console.WriteLine(str);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        Assert.NotNull(company);
        Assert.False(string.IsNullOrWhiteSpace(company.Id));
        Assert.Equal(createDto.Name, company.Name);
        Assert.Equal(createDto.Description, company.Description);
    }

    [Fact]
    public async Task CreateCompany_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CompanyCreateDto
        {
            Name = "NoAuthCo",
            Description = "Should not be created."
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ResourceUrl, createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateCompany_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        // Missing name/description
        var createDto = new CompanyCreateDto { Name = string.Empty, Description = string.Empty };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ResourceUrl, createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetUsersCompany_EnterpriseUser_ReturnsCompany()
    {
        // Arrange: enterprise user seeded with a company
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync(ResourceUrl);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var company = await response.Content.ReadFromJsonAsync<CompanyDto>();
        Assert.NotNull(company);
        Assert.Equal("67a87bdb92156dc8ddd81daa", company.Id);
        Assert.Equal("Tech Corp", company.Name);
        Assert.Equal("A leading tech company.", company.Description);
    }

    [Fact]
    public async Task GetUsersCompany_NonEnterpriseUser_ReturnsForbidden()
    {
        // Arrange: regular test user without enterprise role
        await LoginAsync("test@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync(ResourceUrl);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}