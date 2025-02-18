using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Infrastructure.Services.Identity;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using AssetsManagerApi.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;
using AssetsManagerApi.IntegrationTests.Models;

namespace AssetsManagerApi.IntegrationTests;

public class DbInitializer(CosmosDbContext dbContext)
{
    private readonly CosmosDbContext _dbContext = dbContext;

    public void InitializeDb()
    {
        CleanDatabase().Wait();
        
        InitializeUsersAsync().Wait();
        InitializeCodeAssetsManuallyAsync().Wait();
        // Use only when needed, dont run on every test run due to a big volume of data
        // InitializeCodeAssetsAutomatedAsync().Wait();
    }

    public async Task InitializeUsersAsync()
    {
        #region Roles

        var rolesCollection = await _dbContext.GetContainerAsync("Roles");

        var userRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "User"
        };
        await rolesCollection.CreateItemAsync(userRole);

        var enterpriseRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Enterprise"
        };
        await rolesCollection.CreateItemAsync(enterpriseRole);

        var adminRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Admin"
        };
        await rolesCollection.CreateItemAsync(adminRole);

        #endregion

        #region Users

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));
        var usersCollection = await _dbContext.GetContainerAsync("Users");
        var companiessCollection = await _dbContext.GetContainerAsync("Companies");

        var testUser = new User
        {
            Id = "652c3b89ae02a3135d6409fc",
            Email = "test@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(testUser);

        var updateTestUser = new User
        {
            Id = "672c3b89ae02a3135d6309fc",
            Email = "update@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(updateTestUser);

        var groupUser = new User
        {
            Id = "652c3b89ae02a3135d6439fc",
            Email = "group@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(groupUser);

        var groupUser2 = new User
        {
            Id = "652c3b89ae02a3135d6432fc",
            Email = "group2@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(groupUser2);

        var adminUser = new User
        {
            Id = "652c3b89ae02a3135d6408fc",
            Email = "admin@gmail.com",
            Roles = new List<Role> { userRole, adminRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(adminUser);

        var company = new Company
        {
            Id = "67a87bdb92156dc8ddd81daa",
            Name = "Tech Corp",
            Description = "A leading tech company.",
            CreatedById = adminUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await companiessCollection.CreateItemAsync(company);

        var enterpriseUser = new User
        {
            Id = "6852c3b89ae02a3135d6409fc",
            Email = "enterprise@gmail.com",
            Name = "enterpriseUser",
            Roles = [userRole, enterpriseRole],
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CompanyId = "67a87bdb92156dc8ddd81daa",
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(enterpriseUser);

        var validTokenUser = new User
        {
            Id = "b3e00e4b-4c5a-4e08-932f-5b579d5c3f8f",
            Email = "validtoken@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            EmailVerificationToken = "valid-token",
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(1) // Valid for 1 hour
        };
        await usersCollection.CreateItemAsync(validTokenUser);

        // User with an expired email verification token
        var expiredTokenUser = new User
        {
            Id = "f96ef4c1-6d6e-4eeb-91f1-8f70b3b9e45a",
            Email = "expiredtoken@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            EmailVerificationToken = "expired-token",
            EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(-1) // Expired 1 hour ago
        };
        await usersCollection.CreateItemAsync(expiredTokenUser);

        var noTokenUser = new User
        {
            Id = "d4aeadbb-9c7f-4d2d-9e8a-ffb0f688fdc4",
            Email = "notoken@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            EmailVerificationToken = null,
            EmailVerificationTokenExpiry = null
        };
        await usersCollection.CreateItemAsync(noTokenUser);

        // Users specifically for testing password reset

        var anotherValidResetUser = new User
        {
            Id = "b1d4aead-9c7f-4d2d-9e8a-ffb0f688fabc",
            Email = "anotherreset@example.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            PasswordResetToken = "another-valid-reset-token",
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1) // Valid for 1 hour
        };
        await usersCollection.CreateItemAsync(anotherValidResetUser);

        var validPasswordResetUser = new User
        {
            Id = "a1e00b4c-4d5a-4e18-932f-5c579e5c3a8f",
            Email = "validreset@example.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            PasswordResetToken = "valid-reset-token",
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1) // Valid for 1 hour
        };
        await usersCollection.CreateItemAsync(validPasswordResetUser);

        var expiredPasswordResetUser = new User
        {
            Id = "c96ef4c1-6d7e-4aeb-91f1-8f70b3b9e75b",
            Email = "expiredreset@example.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            PasswordResetToken = "expired-reset-token",
            PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(-1) // Expired 1 hour ago
        };
        await usersCollection.CreateItemAsync(expiredPasswordResetUser);

        var noPasswordResetTokenUser = new User
        {
            Id = "e5aeadbb-9c7f-4d2d-9e8a-ffb0f688fdc4",
            Email = "nopasswordreset@example.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            PasswordResetToken = null,
            PasswordResetTokenExpiry = null
        };
        await usersCollection.CreateItemAsync(noPasswordResetTokenUser);

        #endregion

        #region RefreshTokens

        var refreshTokensCollection = await _dbContext.GetContainerAsync("RefreshTokens");

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            Token = "test-refresh-token",
            ExpiryDateUTC = DateTime.UtcNow.AddDays(-7),
            CreatedById = testUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await refreshTokensCollection.CreateItemAsync(refreshToken);

        #endregion
    }

    private async Task CleanDatabase()
    {
        var iterator = _dbContext.Db.GetContainerQueryIterator<Microsoft.Azure.Cosmos.ContainerProperties>();
        while (iterator.HasMoreResults)
        {
            foreach (var containerProperties in await iterator.ReadNextAsync())
            {
                var containerName = containerProperties.Id;

                if (string.IsNullOrEmpty(containerName))
                {
                    continue;
                }

                var container = _dbContext.Db.GetContainer(containerName);

                await container.DeleteContainerAsync();
            }
        }
    }

    public async Task InitializeCodeAssetsAsync()
    {
        #region Company
        var companiesCollection = await _dbContext.GetContainerAsync("Companies");
        var digitalBank = new Company
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Digital Bank",
            Description = "All online banking services",
            CreatedById = "placeholder",
            CreatedDateUtc = DateTime.UtcNow
        };
        await companiesCollection.CreateItemAsync(digitalBank);

        #endregion
        
        #region Users

        var rolesCollection = await _dbContext.GetContainerAsync("Roles");
        var enterpriseRole = rolesCollection.GetItemLinqQueryable<Role>(true)
            .Where(r => r.Name == "Enterprise")
            .AsEnumerable()
            .FirstOrDefault();

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));

        var usersCollection = await _dbContext.GetContainerAsync("Users");
        var startProjectUser = new User
        {
            Id = "d3aeadbb-9c1f-4d2d-9e8a-ffb0f688fdc4",
            Email = "start-project@gmail.com",
            Roles = [enterpriseRole],
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            EmailVerificationToken = null,
            EmailVerificationTokenExpiry = null,
            CompanyId = digitalBank.Id
        };
        await usersCollection.CreateItemAsync(startProjectUser);

        var noCompanyUser = new User
        {
            Id = "d2aeadbb-9c1f-4d2d-9e1a-ffb0f688fdc4",
            Email = "no-company@gmail.com",
            Roles = [enterpriseRole],
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow,
            EmailVerificationToken = null,
            EmailVerificationTokenExpiry = null
        };
        await usersCollection.CreateItemAsync(noCompanyUser);

        string csvFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "10_digital_bank_users.csv");
        var users = ReadUsersFromCsv(csvFilePath);
        foreach (var user in users)
        {
            user.Roles = [enterpriseRole];
            user.PasswordHash = passwordHasher.Hash("Yuiop12345");
            user.CompanyId = digitalBank.Id;
            user.CreatedById = string.Empty;
            user.CreatedDateUtc = DateTime.UtcNow;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await usersCollection.CreateItemAsync(user);
        }

        #endregion

        #region Tags
        var tagsCollection = await _dbContext.GetContainerAsync("Tags");

        string tagsCsvFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "20_digital_bank_tags.csv");
        List<Tag> tags = ReadTagsFromCsv(tagsCsvFilePath);

        foreach (var tag in tags)
        {
            tag.UseCount = 0;
            await tagsCollection.CreateItemAsync(tag);
        }
        #endregion

        #region Folders / CodeFiles
        
        string jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "file_system_nodes_integration_tests.json");
        var rootFolders = ReadFileSystemNodesFromJson(jsonFilePath);

        foreach (var rootFolder in rootFolders)
        {
            await ProcessNestedItemAsync(rootFolder);
        }

        #endregion 

        #region CodeAssets
        var codeAssetsCollection = await _dbContext.GetContainerAsync("CodeAssets");

        string codeAssetsJsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "code_assets_integration_tests.json");
        var codeAssets = ReadCodeAssetsFromJson(codeAssetsJsonFilePath);

        foreach (var codeAsset in codeAssets)
        {
            codeAsset.CompanyId = digitalBank.Id;
            codeAsset.AssetType = AssetTypes.Corporate;
            await codeAssetsCollection.CreateItemAsync(codeAsset);
        }

        #endregion

        #region StartProjects

        var foldersCollection = await _dbContext.GetContainerAsync("Folders");
        var codeFilesCollection = await _dbContext.GetContainerAsync("CodeFiles");

        var rootFolder3 = new Folder
        {
            Id = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688aac5",
            Name = "Web Development",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = startProjectUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFolder3_1 = new Folder
        {
            Id = "f85eafbb-9c1f-4d2d-9e8a-ffb0f688aac5",
            Name = "Subfolder1",
            ParentId = rootFolder3.Id,
            Type = FileType.Folder,
            CreatedById = startProjectUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile3_1 = new CodeFile
        {
            Id = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f618aac0",
            Name = "Web_Development_Sub1.cs",
            Text = "// Code for Web Development in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = rootFolder3.Id,
            CreatedById = startProjectUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile3_2 = new CodeFile
        {
            Id = "d3faafbb-9c1f-4d2d-9e8a-ffb0f618aac0",
            Name = "Web_Development_Sub1.cs",
            Text = "// Code for Web Development in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = rootFolder3.Id,
            CreatedById = startProjectUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile3_1);
        await codeFilesCollection.CreateItemAsync(subFile3_2);
        await foldersCollection.CreateItemAsync(subFolder3_1);
        await foldersCollection.CreateItemAsync(rootFolder3);

        var asset = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Start Project",
            Description = "Start project",
            AssetType = AssetTypes.StartProject,
            Language = Languages.csharp,
            RootFolderId = rootFolder3.Id,
            PrimaryCodeFileId = subFile3_1.Id,
            Tags = [],
            CreatedById = startProjectUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset);

        var startProjectsCollection = await _dbContext.GetContainerAsync("StartProjects");

        var startProject = new StartProject
        {
            Id = "d3ceafbb-9c1f-4d2d-9e8a-ffb0f688fdc4",
            CodeAssetsIds = [..codeAssets.Select(x => x.Id)],
            CompanyId = digitalBank.Id,
            CreatedDateUtc = DateTime.UtcNow,
            CreatedById = startProjectUser.Id,
            CodeAssetId = asset.Id
        };
        await startProjectsCollection.CreateItemAsync(startProject);

        #endregion
    }

    /// <summary>
    /// Add code assets to the database from Digital Bank company 
    /// Do not use for integration tests, only for initial seeding
    /// </summary>
    public async Task InitializeCodeAssetsAutomatedAsync()
    {
        #region Company
        var companiesCollection = await _dbContext.GetContainerAsync("Companies");
        var digitalBank = new Company
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Digital Bank",
            Description = "All online banking services",
            CreatedById = "placeholder",
            CreatedDateUtc = DateTime.UtcNow
        };
        await companiesCollection.CreateItemAsync(digitalBank);

        #endregion
        
        #region Users

        var rolesCollection = await _dbContext.GetContainerAsync("Roles");
        var enterpriseRole = rolesCollection.GetItemLinqQueryable<Role>(true)
            .Where(r => r.Name == "Enterprise")
            .AsEnumerable()
            .FirstOrDefault();

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));

        var usersCollection = await _dbContext.GetContainerAsync("Users");

        string csvFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "10_digital_bank_users.csv");
        var users = ReadUsersFromCsv(csvFilePath);
        foreach (var user in users)
        {
            user.Roles = [enterpriseRole];
            user.PasswordHash = passwordHasher.Hash("Yuiop12345");
            user.CompanyId = digitalBank.Id;
            user.CreatedById = string.Empty;
            user.CreatedDateUtc = DateTime.UtcNow;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            await usersCollection.CreateItemAsync(user);
        }

        #endregion

        #region Tags
        var tagsCollection = await _dbContext.GetContainerAsync("Tags");

        string tagsCsvFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "20_digital_bank_tags.csv");
        List<Tag> tags = ReadTagsFromCsv(tagsCsvFilePath);

        foreach (var tag in tags)
        {
            tag.UseCount = 0;
            await tagsCollection.CreateItemAsync(tag);
        }
        #endregion

        #region Folders / CodeFiles
        
        string jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "FileSystemNodes.json");
        var rootFolders = ReadFileSystemNodesFromJson(jsonFilePath);

        foreach (var rootFolder in rootFolders)
        {
            await ProcessNestedItemAsync(rootFolder);
        }

        #endregion 

        #region CodeAssets
        var codeAssetsCollection = await _dbContext.GetContainerAsync("CodeAssets");

        string codeAssetsJsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "CodeAssets.json");
        var codeAssets = ReadCodeAssetsFromJson(codeAssetsJsonFilePath);

        // Add each code asset to the database
        foreach (var codeAsset in codeAssets)
        {
            codeAsset.CompanyId = digitalBank.Id;
            await codeAssetsCollection.CreateItemAsync(codeAsset);
        }

        #endregion
    }

    public async Task InitializeCodeAssetsManuallyAsync()
    {
        var tagsCollection = await _dbContext.GetContainerAsync("Tags");
        var codeAssetsCollection = await _dbContext.GetContainerAsync("CodeAssets");
        var foldersCollection = await _dbContext.GetContainerAsync("Folders");
        var codeFilesCollection = await _dbContext.GetContainerAsync("CodeFiles");
        var usersCollection = await _dbContext.GetContainerAsync("Users");

        var admin = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                                   .Where(u => u.Id == "652c3b89ae02a3135d6408fc")
                                   .AsEnumerable()
                                   .FirstOrDefault();

        var enterpriseUser = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                                            .Where(u => u.Id == "6852c3b89ae02a3135d6409fc")
                                            .AsEnumerable()
                                            .FirstOrDefault();

        #region Tags

        var tags = new List<Tag>
        {
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Csharp", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "JavaScript", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Python", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Web Development", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = "67a806cefde1b0618b381fd6", Name = "Machine Learning", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 2 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Database", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Mobile Development", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "DevOps", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Cloud Computing", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Big Data", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Data Science", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Artificial Intelligence", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Cyber Security", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Networking", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Blockchain", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "AR/VR", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Game Development", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Robotics", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Embedded Systems", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "IoT", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Automation", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Testing", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Performance Optimization", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Code Review", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "CI/CD", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Documentation", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Version Control", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Open Source", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "API", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "UI UX", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Design Patterns", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0}
        };

        foreach (var tag in tags)
        {
            await tagsCollection.CreateItemAsync(tag);
        }

        #endregion

        #region CodeAssets

        // Create root folder 1
        var rootFolder1 = new Folder
        {
            Id = "67a92a35076f3e883da979ee",
            Name = "Machine Learning",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 1
        var subFolder1_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder1.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile1_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Machine_Learning_Sub1.cs",
            Text = "// Code for Machine Learning in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder1_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile1_1);
        await foldersCollection.CreateItemAsync(subFolder1_1);
        await foldersCollection.CreateItemAsync(rootFolder1);

        // Create code asset 1
        var asset1 = new CodeAsset
        {
            Id = "67a767a843d60f5e4add55c9",
            Name = "Machine Learning Project",
            Description = "Python machine learning model",
            AssetType = AssetTypes.Corporate,
            CompanyId = enterpriseUser.CompanyId,
            Language = Languages.python,
            RootFolderId = rootFolder1.Id,
            PrimaryCodeFileId = subFile1_1.Id,
            Tags = new List<Tag> { tags[4] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset1);

        // Create root folder 2
        var rootFolder2 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Database Scripts",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 2
        var subFolder2_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder2.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile2_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Database_Scripts_Sub1.cs",
            Text = "// Code for Database Scripts in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder2_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile2_1);
        await foldersCollection.CreateItemAsync(subFolder2_1);
        await foldersCollection.CreateItemAsync(rootFolder2);

        // Create code asset 2
        var asset2 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Database Management",
            Description = "SQL database scripts",
            AssetType = AssetTypes.Public,
            Language = Languages.csharp,
            RootFolderId = rootFolder2.Id,
            PrimaryCodeFileId = subFile2_1.Id,
            Tags = new List<Tag> { tags[5] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset2);

        // Create root folder 3
        var rootFolder3 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Web Development",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 3
        var subFolder3_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder3.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile3_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Web_Development_Sub1.cs",
            Text = "// Code for Web Development in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder3_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile3_1);
        await foldersCollection.CreateItemAsync(subFolder3_1);
        await foldersCollection.CreateItemAsync(rootFolder3);

        // Create code asset 3
        var asset3 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Web Application",
            Description = "JavaScript frontend development",
            AssetType = AssetTypes.Public,
            Language = Languages.javascript,
            RootFolderId = rootFolder3.Id,
            PrimaryCodeFileId = subFile3_1.Id,
            Tags = new List<Tag> { tags[3] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset3);

        // Create root folder 4
        var rootFolder4 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Mobile App",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 4
        var subFolder4_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder4.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile4_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Mobile_App_Sub1.cs",
            Text = "// Code for Mobile App in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder4_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile4_1);
        await foldersCollection.CreateItemAsync(subFolder4_1);
        await foldersCollection.CreateItemAsync(rootFolder4);

        // Create code asset 4
        var asset4 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Mobile Application",
            Description = "Kotlin mobile app development",
            AssetType = AssetTypes.Corporate,
            CompanyId = enterpriseUser.CompanyId,
            Language = Languages.python,
            RootFolderId = rootFolder4.Id,
            PrimaryCodeFileId = subFile4_1.Id,
            Tags = new List<Tag> { tags[2] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset4);

        // Create root folder 5
        var rootFolder5 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game Development",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 5
        var subFolder5_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder5.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile5_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game_Development_Sub1.cs",
            Text = "// Code for Game Development in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder5_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile5_1);
        await foldersCollection.CreateItemAsync(subFolder5_1);
        await foldersCollection.CreateItemAsync(rootFolder5);

        // Create code asset 5
        var asset5 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Game Development Project",
            Description = "C# game development project",
            AssetType = AssetTypes.Corporate,
            CompanyId = enterpriseUser.CompanyId,
            Language = Languages.csharp,
            RootFolderId = rootFolder5.Id,
            PrimaryCodeFileId = subFile5_1.Id,
            Tags = new List<Tag> { tags[0] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset5);

        // Create root folder 6
        var rootFolder6 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AI Research",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        // Add subfolders and files to root folder 6
        var subFolder6_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Subfolder1",
            ParentId = rootFolder6.Id,
            Type = FileType.Folder,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        var subFile6_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AI_Research_Sub1.cs",
            Text = "// Code for AI Research in Subfolder1",
            Language = Languages.csharp,
            Type = FileType.CodeFile,
            ParentId = subFolder6_1.Id,
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeFilesCollection.CreateItemAsync(subFile6_1);
        await foldersCollection.CreateItemAsync(subFolder6_1);
        await foldersCollection.CreateItemAsync(rootFolder6);

        // Create code asset 6
        var asset6 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AI Research Project",
            Description = "AI research data and models",
            AssetType = AssetTypes.Public,
            Language = Languages.csharp,
            RootFolderId = rootFolder6.Id,
            PrimaryCodeFileId = subFile6_1.Id,
            Tags = new List<Tag> { tags[4] },
            CreatedById = enterpriseUser.Id,
            CreatedDateUtc = DateTime.UtcNow
        };

        await codeAssetsCollection.CreateItemAsync(asset6);

        #endregion
    }


    private static List<CodeAsset> ReadCodeAssetsFromJson(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<CodeAsset>>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<CodeAsset>();
    }

    private static List<User> ReadUsersFromCsv(string filePath)
    {
        var users = new List<User>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        }))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var user = new User
                {
                    Id = csv.GetField("Id"),
                    Name = csv.GetField("Name"),
                    Email = csv.GetField("Email")
                };
                users.Add(user);
            }
        }

        return users;
    }

    private static List<Tag> ReadTagsFromCsv(string filePath)
    {
        var tags = new List<Tag>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        }))
        {
            csv.Read();        // Move to the first row
            csv.ReadHeader();  // Read the header row

            while (csv.Read())
            {
                var tag = new Tag
                {
                    Id = csv.GetField("Id"),
                    Name = csv.GetField("Name")
                };
                tags.Add(tag);
            }
        }

        return tags;
    }

    private static List<FolderDataSeeding> ReadFileSystemNodesFromJson(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new FileSystemNodeConverter() }
        };

        return JsonSerializer.Deserialize<List<FolderDataSeeding>>(jsonContent, options) ?? new List<FolderDataSeeding>();
    }

    private async Task ProcessNestedItemAsync(FileSystemNode item)
    {
        var foldersCollection = await _dbContext.GetContainerAsync("Folders");
        var codeFilesCollection = await _dbContext.GetContainerAsync("CodeFiles");

        switch (item.Type)
        {
            case FileType.Folder:
                // Cast to Folder and insert into Folders collection
                var subFolder = (FolderDataSeeding)item;
                var subFolderCopy = new Folder
                {
                    Id = subFolder.Id,
                    Name = subFolder.Name,
                    ParentId = subFolder.ParentId,
                    Type = subFolder.Type,
                    // Items should not be included in the database
                };
                await foldersCollection.CreateItemAsync(subFolderCopy);
                Console.WriteLine($"Added subfolder: {subFolder.Name}");

                // Recursively process subfolder items
                foreach (var nestedItem in subFolder.Items)
                {
                    Console.WriteLine($"Processing nested item: {nestedItem.Name}");
                    await ProcessNestedItemAsync(nestedItem);
                }

                break;

            case FileType.CodeFile:
                // Cast to CodeFile and insert into CodeFiles collection
                var codeFile = new CodeFile
                {
                    Id = item.Id,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    Type = item.Type,
                    Text = ((CodeFile)item).Text,
                    Language = ((CodeFile)item).Language
                };
                await codeFilesCollection.CreateItemAsync(codeFile);
                Console.WriteLine($"Added code file: {codeFile.Name}");
                break;

            default:
                Console.WriteLine($"Unknown file type for item: {item.Name}");
                break;
        }
    }
    
    public class FileSystemNodeConverter : JsonConverter<FileSystemNode>
    {
        public override FileSystemNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Parse the JSON object
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                JsonElement root = document.RootElement;
                FileType type = (FileType)root.GetProperty("Type").GetInt32();

                // Deserialize based on the type
                if (type == FileType.Folder)
                {
                    return JsonSerializer.Deserialize<FolderDataSeeding>(root.GetRawText(), options);
                }
                else if (type == FileType.CodeFile)
                {
                    return JsonSerializer.Deserialize<CodeFile>(root.GetRawText(), options);
                }
                else
                {
                    throw new NotSupportedException($"Unsupported file type: {type}");
                }
            }
        }

        public override void Write(Utf8JsonWriter writer, FileSystemNode value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}