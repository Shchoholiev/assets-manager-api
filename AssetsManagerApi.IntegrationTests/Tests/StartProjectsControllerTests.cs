using System.Net;
using System.Net.Http.Json;
using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.UpdateDto;

namespace AssetsManagerApi.IntegrationTests.Tests;

public class StartProjectsControllerTests(TestingFactory<Program> factory) 
    : TestsBase(factory, "start-projects")
{
    #region CreateStartProject

    [Fact]
    public async Task CreateStartProject_ValidInput_ReturnsAssets()
    {
       // Arrange
       var request = new StartProjectCreateDto
       {
           Prompt = "I'm building a virtual assistant feature to handle customer inquiries with " + 
                    "an AI-driven solution, integrating chat support for smoother interaction."
       };
       await LoginAsync("start-project@gmail.com", "Yuiop12345");

       // Act
       var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);
       var startProject = await response.Content.ReadFromJsonAsync<StartProjectDto>();

       // Assert
       Assert.Equal(HttpStatusCode.OK, response.StatusCode);
       Assert.NotNull(startProject);
       Assert.NotNull(startProject.CodeAssets);
        
       // Assert that there are exactly 2 assets
       Assert.Equal(2, startProject.CodeAssets.Count);

       // Assert that the assets have the expected IDs
       Assert.Contains(startProject.CodeAssets, asset => asset.Id.ToString() == "4b32587b-5e64-435c-9ba8-69740845bd5a");
       Assert.Contains(startProject.CodeAssets, asset => asset.Id.ToString() == "c4c8ec92-71cb-46d8-9b65-97c21aa6681f");
    }

    [Fact]
    public async Task CreateStartProject_UserWithoutCompany_Returns403Forbidden()
    {
        // Arrange
        var request = new StartProjectCreateDto
        {
            Prompt = "I'm building a virtual assistant feature to handle customer inquiries with " + 
                     "an AI-driven solution, integrating chat support for smoother interaction."
        };
        await LoginAsync("no-company@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateStartProject_MissingPrompt_Returns400BadRequest()
    {
        // Arrange
        var request = new StartProjectCreateDto();
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStartProject_NotAuthorized_Returns401Unauthorized()
    {
        // Arrange
        var request = new StartProjectCreateDto();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region CombineStartProject

    [Fact]
    public async Task CombineStartProject_ValidId_ReturnsCombinedCodeAsset()
    {
        // Arrange
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        var startProjectId = "b3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        var combineUrl = $"{ResourceUrl}/{startProjectId}/combine";

        // Act
        var response = await HttpClient.PostAsync(combineUrl, null);
        var combinedAsset = await response.Content.ReadFromJsonAsync<CodeAssetDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(combinedAsset);
        Assert.False(string.IsNullOrEmpty(combinedAsset.Id.ToString()));
        Assert.False(string.IsNullOrEmpty(combinedAsset.Name));
        Assert.NotNull(combinedAsset.RootFolder);
    }

    [Fact]
    public async Task CombineStartProject_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        var invalidId = "non-existent-id";
        var combineUrl = $"{ResourceUrl}/{invalidId}/combine";

        // Act
        var response = await HttpClient.PostAsync(combineUrl, null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region CreateCodeFile

    [Fact]
    public async Task CreateCodeFile_ValidInput_Returns201WithCodeFile()
    {
        // Arrange
        var codeFile = new CodeFileCreateDto
        {
            Text = "Console.WriteLine(\"Hello, World!\");",
            Language = "Csharp",
            Name = "HelloWorld.cs",
            ParentId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5"
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{startProjectId}/code-files", codeFile);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCodeFile = await response.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);
        Assert.Equal(codeFile.Name, createdCodeFile.Name);
        Assert.Equal(codeFile.Language, createdCodeFile.Language);
        Assert.Equal(codeFile.Text, createdCodeFile.Text);
    }

    #endregion


    #region UpdateCodeFile

    [Fact]
    public async Task UpdateCodeFile_ValidInput_Returns200WithCodeFile()
    {
        // Arrange
        var codeFileId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f618aac0";
        var codeFile = new CodeFileUpdateDto
        {
            Id = codeFileId,
            Text = "Console.WriteLine(\"Hello, World!\");",
            Language = "Csharp",
            Name = "HelloWorld.cs",
            ParentId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5"
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{startProjectId}/code-files/{codeFileId}", codeFile);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedCodeFile = await response.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(updatedCodeFile);
        Assert.Equal(codeFile.Name, updatedCodeFile.Name);
        Assert.Equal(codeFile.Language, updatedCodeFile.Language);
        Assert.Equal(codeFile.Text, updatedCodeFile.Text);
    }

    #endregion


    #region DeleteCodeFile

    [Fact]
    public async Task DeleteCodeFile_ValidInput_Returns204NoContent()
    {
        // Arrange
        var codeFileId = "d3faafbb-9c1f-4d2d-9e8a-ffb0f618aac0";
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{startProjectId}/code-files/{codeFileId}");
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion


    #region CreateFolder

    [Fact]
    public async Task CreateFolder_ValidInput_Returns201WithFolder()
    {
        // Arrange
        var folder = new FolderCreateDto
        {
            Name = "HelloWorld",
            ParentId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5"
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{startProjectId}/folders", folder);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdFolder = await response.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        Assert.Equal(folder.Name, createdFolder.Name);
    }

    #endregion


    #region UpdateFolder

    [Fact]
    public async Task UpdateFolder_ValidInput_Returns200WithCodeFile()
    {
        // Arrange
        var folderId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5";
        var folder = new FolderUpdateDto
        {
            Id = folderId,
            Name = "HelloWorldRoot",
        };
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{startProjectId}/folders/{folderId}", folder);
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updatedCodeFile = await response.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(updatedCodeFile);
        Assert.Equal(folder.Name, updatedCodeFile.Name);
    }

    #endregion


    #region DeleteFolder

    [Fact]
    public async Task DeleteFolder_ValidInput_Returns204NoContent()
    {
        // Arrange
        var folderId = "f85eafbb-9c1f-4d2d-9e8a-ffb0f688aac5";
        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{startProjectId}/folders/{folderId}");
        var stringContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response content: {stringContent}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    #endregion

    #region DownloadStartProjectZip

    [Fact]
    public async Task DownloadStartProjectZip_ValidId_ReturnsZipFile()
    {
        // Arrange
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        var startProjectId = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4";
        var combineUrl = $"{ResourceUrl}/{startProjectId}/download";

        // Act
        var response = await HttpClient.GetAsync(combineUrl);
        var content = await response.Content.ReadAsByteArrayAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task DownloadStartProjectZip_ValidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("start-project@gmail.com", "Yuiop12345");

        var invalidId = "non-existent-id";
        var combineUrl = $"{ResourceUrl}/{invalidId}/combine";

        // Act
        var response = await HttpClient.PostAsync(combineUrl, null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}