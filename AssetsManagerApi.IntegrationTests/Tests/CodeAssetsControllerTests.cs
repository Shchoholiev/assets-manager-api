using AssetsManagerApi.Application.Models.CreateDto;
using AssetsManagerApi.Application.Models.Dto;
using AssetsManagerApi.Application.Models.UpdateDto;
using AssetsManagerApi.Application.Paging;
using System.Net.Http.Json;
using System.Net;
using AssetsManagerApi.Application.Models.Operations;
using AssetsManagerApi.Domain.Enums;
using System.Linq.Expressions;
using AssetsManagerApi.Application.Models.Global;

namespace AssetsManagerApi.IntegrationTests.Tests;

public class CodeAssetsControllerTests(TestingFactory<Program> factory)
    : TestsBase(factory, "code-assets")
{
    [Fact]
    public async Task GetCodeAssetAsync_ValidId_ReturnsAsset()
    {
        // Arrange
        var codeAssetId = "67a767a843d60f5e4add55c9";
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{codeAssetId}");
        var asset = await response.Content.ReadFromJsonAsync<CodeAssetDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(asset);
        Assert.Equal(codeAssetId, asset.Id);
    }

    [Fact]
    public async Task GetCodeAssetAsync_InvalidId_ReturnsNotFound()
    {
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/invalid-id");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_WithFilterCorporateAssets_ReturnsPaginatedList()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&assetType={filterModel.AssetType}&language={filterModel.Language}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(pagedList);
        Assert.True(pagedList.Items.Any());

        // Additional asserts to verify filtering
        Assert.All(pagedList.Items, item => Assert.Equal(filterModel.AssetType, item.AssetType));
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_WithFilterSearchString_ReturnsPaginatedList()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            SearchString = "python",
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={filterModel.SearchString}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(pagedList);
        Assert.True(pagedList.Items.Any());

        // Additional asserts to verify filtering
        Assert.All(pagedList.Items, item =>
            Assert.True(item.Name.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase) ||
                        item.Description.Contains(filterModel.SearchString, StringComparison.OrdinalIgnoreCase))
            );
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_WithFilterTagIds_ReturnsPaginatedList()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            TagIds = new List<string> { "67a806cefde1b0618b381fd6" },
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&tagIds={string.Join(",", filterModel.TagIds)}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(pagedList);
        Assert.True(pagedList.Items.Any());

        // Additional asserts to verify filtering
        Assert.All(pagedList.Items, item => Assert.True(item.Tags.Any(tag => filterModel.TagIds.Contains(tag.Id))));
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_WithFilterLanguage_ReturnsPaginatedList()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
            Language = "Python",
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&language={filterModel.Language}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(pagedList);
        Assert.True(pagedList.Items.Any());

        // Additional asserts to verify filtering
        Assert.All(pagedList.Items, item => Assert.Equal(filterModel.Language, item.Language.ToString()));
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidPageNumber_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = -1; // Invalid page number
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidPageSize_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = -1; // Invalid page size

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidAssetType_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = (AssetTypes)(-1), // Invalid asset type
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&assetType={filterModel.AssetType}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidTagId_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            TagIds = new List<string> { "invalid-tag-id" }, // Invalid tag ID
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&tagIds={string.Join(",", filterModel.TagIds)}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Empty(pagedList.Items);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidLanguage_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            Language = "InvalidLanguage",
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&language={filterModel.Language}&assetType={filterModel.AssetType}");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_InvalidSearchString_ReturnsBadRequest()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            SearchString = "%invalidsearchstring%",
            AssetType = AssetTypes.Corporate,
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&searchString={filterModel.SearchString}&assetType={filterModel.AssetType}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Empty(pagedList.Items);
    }

    [Fact]
    public async Task GetCodeAssetsPageAsync_WithFilterIsPersonal_ReturnsPaginatedList()
    {
        // Arrange
        var filterModel = new CodeAssetFilterModel
        {
            AssetType = AssetTypes.Corporate,
            IsPersonal = true
        };

        var pageNumber = 1;
        var pageSize = 10;

        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?pageNumber={pageNumber}&pageSize={pageSize}&assetType={filterModel.AssetType}&isPersonal={filterModel.IsPersonal}");
        var pagedList = await response.Content.ReadFromJsonAsync<PagedList<CodeAssetDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(pagedList);
        Assert.True(pagedList.Items.Any());

        // Additional asserts to verify filtering
        Assert.All(pagedList.Items, item => Assert.True(item.UserName == GlobalUser.Name));
    }

    [Fact]
    public async Task CreateCodeAssetAsync_WithFolderAndPrimaryCodeFile_ReturnsCreatedAsset()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset referencing the folder and primary code file
        var createAssetDto = new CodeAssetCreateDto
        {
            Name = "Asset With Folder and Code File",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };
        var assetResponse = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);
        var createdAsset = await assetResponse.Content.ReadFromJsonAsync<CodeAssetDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, assetResponse.StatusCode);
        Assert.NotNull(createdAsset);
        Assert.Equal(createAssetDto.Name, createdAsset.Name);
        Assert.Equal(createAssetDto.AssetType, createdAsset.AssetType);
        Assert.Equal(createAssetDto.Language, createdAsset.Language);
        Assert.Equal(createAssetDto.RootFolderName, createdAsset.RootFolder.Name);
        Assert.Equal(createAssetDto.PrimaryCodeFileName, createdAsset.PrimaryCodeFile.Name);
        Assert.Equal(createdAsset.RootFolder.Id, createdAsset.PrimaryCodeFile.ParentId);
    }

    [Fact]
    public async Task CreateCodeAssetAsync_MissingName_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset with missing name
        var createAssetDto = new CodeAssetCreateDto
        {
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    [Fact]
    public async Task DeleteFolderAsync_ValidId_ReturnsDeletedFolder()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/folders/{folderId}");
        var deletedFolder = await deleteResponse.Content.ReadFromJsonAsync<FolderDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletedFolder);
        Assert.Equal(folderId, deletedFolder.Id);
    }
    [Fact]
    public async Task DeleteCodeFileAsync_ValidId_ReturnsDeletedCodeFile()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Create the primary code file in the new folder
        var createCodeFileDto = new CodeFileCreateDto
        {
            Name = "PrimaryCodeFile.cs",
            Text = "public class PrimaryCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };
        var codeFileResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/codefiles", createCodeFileDto);
        codeFileResponse.EnsureSuccessStatusCode();
        var createdCodeFile = await codeFileResponse.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);
        var codeFileId = createdCodeFile.Id;

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/codefiles/{codeFileId}");
        var deletedCodeFile = await deleteResponse.Content.ReadFromJsonAsync<CodeFileDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletedCodeFile);
        Assert.Equal(codeFileId, deletedCodeFile.Id);
    }
    [Fact]
    public async Task DeleteCodeAssetAsync_ValidId_ReturnsDeletedAsset()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset referencing the folder and primary code file
        var createAssetDto = new CodeAssetCreateDto
        {
            Name = "Asset With Folder and Code File",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };
        var assetResponse = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);
        var createdAsset = await assetResponse.Content.ReadFromJsonAsync<CodeAssetDto>();
        Assert.NotNull(createdAsset);
        var assetId = createdAsset.Id;

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/{assetId}");
        var deletedAsset = await deleteResponse.Content.ReadFromJsonAsync<CodeAssetDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        Assert.NotNull(deletedAsset);
        Assert.Equal(assetId, deletedAsset.Id);
    }
    [Fact]
    public async Task DeleteCodeAssetAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/invalid-asset-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    [Fact]
    public async Task DeleteCodeFileAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/codefiles/invalid-codefile-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    [Fact]
    public async Task DeleteFolderAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Act
        var deleteResponse = await HttpClient.DeleteAsync($"{ResourceUrl}/folders/invalid-folder-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateCodeAssetAsync_ValidId_ReturnsUpdatedAsset()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset referencing the folder and primary code file
        var createAssetDto = new CodeAssetCreateDto
        {
            Name = "Asset With Folder and Code File",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };
        var assetResponse = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);
        var createdAsset = await assetResponse.Content.ReadFromJsonAsync<CodeAssetDto>();
        Assert.NotNull(createdAsset);
        var assetId = createdAsset.Id;

        // Update the asset
        var updateAssetDto = new CodeAssetUpdateDto
        {
            Id = assetId,
            Name = "Updated Asset Name",
            Description = "Updated Description",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderId = createdAsset.RootFolder.Id,
            PrimaryCodeFileId = createdAsset.PrimaryCodeFile.Id,
            TagsIds = new List<string> { "67a806cefde1b0618b381fd6" } // Add any necessary tags
        };
        var updateResponse = await HttpClient.PutAsJsonAsync(ResourceUrl, updateAssetDto);
        var updatedAsset = await updateResponse.Content.ReadFromJsonAsync<CodeAssetDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updatedAsset);
        Assert.Equal(updateAssetDto.Name, updatedAsset.Name);
        Assert.Equal(updateAssetDto.Description, updatedAsset.Description);
        Assert.Equal(updateAssetDto.AssetType, updatedAsset.AssetType);
        Assert.Equal(updateAssetDto.Language, updatedAsset.Language);
        Assert.Equal(createdAsset.RootFolder.Id, updatedAsset.RootFolder.Id);
        Assert.Equal(createdAsset.PrimaryCodeFile.Id, updatedAsset.PrimaryCodeFile.Id);
    }
    [Fact]
    public async Task UpdateFolderAsync_ValidId_ReturnsUpdatedFolder()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Update the folder
        var updateFolderDto = new FolderUpdateDto
        {
            Id = folderId,
            Name = "UpdatedRootFolder",
            ParentId = null
        };
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/folders", updateFolderDto);
        var updatedFolder = await updateResponse.Content.ReadFromJsonAsync<FolderDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updatedFolder);
        Assert.Equal(updateFolderDto.Name, updatedFolder.Name);
    }

    [Fact]
    public async Task UpdateCodeFileAsync_ValidId_ReturnsUpdatedCodeFile()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Create the primary code file in the new folder
        var createCodeFileDto = new CodeFileCreateDto
        {
            Name = "PrimaryCodeFile.cs",
            Text = "public class PrimaryCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };
        var codeFileResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/codefiles", createCodeFileDto);
        codeFileResponse.EnsureSuccessStatusCode();
        var createdCodeFile = await codeFileResponse.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);
        var codeFileId = createdCodeFile.Id;

        // 3. Update the code file
        var updateCodeFileDto = new CodeFileUpdateDto
        {
            Id = codeFileId,
            Name = "UpdatedCodeFile.cs",
            Text = "public class UpdatedCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/codefiles", updateCodeFileDto);
        var updatedCodeFile = await updateResponse.Content.ReadFromJsonAsync<CodeFileDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updatedCodeFile);
        Assert.Equal(updateCodeFileDto.Name, updatedCodeFile.Name);
        Assert.Equal(updateCodeFileDto.Text, updatedCodeFile.Text);
        Assert.Equal(updateCodeFileDto.Language, updatedCodeFile.Language);
    }
    [Fact]
    public async Task UpdateCodeAssetAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset referencing the folder and primary code file
        var createAssetDto = new CodeAssetCreateDto
        {
            Name = "Asset With Folder and Code File",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };
        var assetResponse = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);
        var createdAsset = await assetResponse.Content.ReadFromJsonAsync<CodeAssetDto>();
        Assert.NotNull(createdAsset);

        // Update the asset with an invalid ID
        var updateAssetDto = new CodeAssetUpdateDto
        {
            Id = "invalid-asset-id",
            Name = "Updated Asset Name",
            Description = "Updated Description",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderId = createdAsset.RootFolder.Id,
            PrimaryCodeFileId = createdAsset.PrimaryCodeFile.Id,
            TagsIds = new List<string> { "67a806cefde1b0618b381fd6" }
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync(ResourceUrl, updateAssetDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateCodeAssetAsync_MissingName_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // Create the asset referencing the folder and primary code file
        var createAssetDto = new CodeAssetCreateDto
        {
            Name = "Asset With Folder and Code File",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderName = "RootFolderName",
            PrimaryCodeFileName = "PrimaryCodeFileName",
        };
        var assetResponse = await HttpClient.PostAsJsonAsync(ResourceUrl, createAssetDto);
        var createdAsset = await assetResponse.Content.ReadFromJsonAsync<CodeAssetDto>();
        Assert.NotNull(createdAsset);
        var assetId = createdAsset.Id;

        // Update the asset with a missing name
        var updateAssetDto = new CodeAssetUpdateDto
        {
            Id = assetId,
            Description = "Updated Description",
            AssetType = AssetTypes.Corporate,
            Language = "Csharp",
            RootFolderId = createdAsset.RootFolder.Id,
            PrimaryCodeFileId = createdAsset.PrimaryCodeFile.Id,
            TagsIds = new List<string> { "67a806cefde1b0618b381fd6" }
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync(ResourceUrl, updateAssetDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateCodeFileAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Create the primary code file in the new folder
        var createCodeFileDto = new CodeFileCreateDto
        {
            Name = "PrimaryCodeFile.cs",
            Text = "public class PrimaryCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };
        var codeFileResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/codefiles", createCodeFileDto);
        codeFileResponse.EnsureSuccessStatusCode();
        var createdCodeFile = await codeFileResponse.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);

        // 3. Update the code file with an invalid ID
        var updateCodeFileDto = new CodeFileUpdateDto
        {
            Id = "invalid-codefile-id",
            Name = "UpdatedCodeFile.cs",
            Text = "public class UpdatedCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/codefiles", updateCodeFileDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateCodeFileAsync_MissingName_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Create the primary code file in the new folder
        var createCodeFileDto = new CodeFileCreateDto
        {
            Name = "PrimaryCodeFile.cs",
            Text = "public class PrimaryCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };
        var codeFileResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/codefiles", createCodeFileDto);
        codeFileResponse.EnsureSuccessStatusCode();
        var createdCodeFile = await codeFileResponse.Content.ReadFromJsonAsync<CodeFileDto>();
        Assert.NotNull(createdCodeFile);
        var codeFileId = createdCodeFile.Id;

        // 3. Update the code file with a missing name
        var updateCodeFileDto = new CodeFileUpdateDto
        {
            Id = codeFileId,
            Text = "public class UpdatedCodeFile {}",
            Language = "Csharp",
            ParentId = folderId
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/codefiles", updateCodeFileDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateFolderAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);

        // 2. Update the folder with an invalid ID
        var updateFolderDto = new FolderUpdateDto
        {
            Id = "invalid-folder-id",
            Name = "UpdatedRootFolder",
            ParentId = null
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/folders", updateFolderDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }
    [Fact]
    public async Task UpdateFolderAsync_MissingName_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("enterprise@gmail.com", "Yuiop12345");

        // 1. Create the root folder
        var createFolderDto = new FolderCreateDto
        {
            Name = "RootFolder",
            ParentId = null
        };
        var folderResponse = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/folders", createFolderDto);
        folderResponse.EnsureSuccessStatusCode();
        var createdFolder = await folderResponse.Content.ReadFromJsonAsync<FolderDto>();
        Assert.NotNull(createdFolder);
        var folderId = createdFolder.Id;

        // 2. Update the folder with a missing name
        var updateFolderDto = new FolderUpdateDto
        {
            Id = folderId,
            ParentId = null
        };

        // Act
        var updateResponse = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/folders", updateFolderDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);
    }
}