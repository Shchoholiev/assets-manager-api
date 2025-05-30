﻿using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Domain.Enums;
using AssetsManagerApi.Infrastructure.Services.Identity;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Extensions.Logging;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.Json;
using AssetsManagerApi.Persistance.Models;
using System.Text.Json.Serialization;

namespace AssetsManagerApi.Persistance.PersistanceExtentions;

public class DbInitializer(CosmosDbContext dbContext)
{
    private readonly CosmosDbContext _dbContext = dbContext;

    public void InitializeDb()
    {
        CleanDatabase().Wait();

        InitializeUsersAsync().Wait();
        InitializeCompaniesAsync().Wait();
        InitializeCodeAssetsAsync().Wait();
        InitializeCodeAssetsForStartProjectsAsync().Wait();
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

        var adminRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Admin"
        };
        await rolesCollection.CreateItemAsync(adminRole);

        var enterpriseRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Enterprise"
        };
        await rolesCollection.CreateItemAsync(enterpriseRole);

        var guestRole = new Role
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Guest"
        };
        await rolesCollection.CreateItemAsync(guestRole);

        #endregion

        #region Users

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));
        var usersCollection = await _dbContext.GetContainerAsync("Users");

        var testUser = new User
        {
            Id = "652c3b89ae02a3135d6409fc",
            Name = "testUser",
            Email = "test@gmail.com",
            Roles = new List<Role> { userRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(testUser);

        var enterpriseUser = new User
        {
            Id = "679e7985716a8576abe04445",
            Name = "enterpriseUser",
            Email = "enterprise@gmail.com",
            Roles = new List<Role> { userRole, enterpriseRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(enterpriseUser);

        var guestUser = new User
        {
            Id = "679e799ed675c86f0b45581e",
            Name = "guestUser",
            Email = "guest@gmail.com",
            Roles = new List<Role> { guestRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(guestUser);

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
            Name = "admin",
            Roles = new List<Role> { userRole, adminRole },
            PasswordHash = passwordHasher.Hash("Yuiop12345"),
            CreatedById = string.Empty,
            CreatedDateUtc = DateTime.UtcNow
        };
        await usersCollection.CreateItemAsync(adminUser);

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

    public async Task InitializeCodeAssetsAsync()
    {
        var usersCollection = await _dbContext.GetContainerAsync("Users");

        var admin = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                                   .Where(u => u.Id == "652c3b89ae02a3135d6408fc")
                                   .AsEnumerable()
                                   .FirstOrDefault();

        #region Tags

        var tagsCollection = await _dbContext.GetContainerAsync("Tags");

        var tags = new List<Tag>
        {
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Web Development", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Machine Learning", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 2 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Database", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 1 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Mobile Development", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "DevOps", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Cloud Computing", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Big Data", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Data Science", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Artificial Intelligence", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Cyber Security", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Networking", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 2 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Blockchain", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Automation", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "Testing", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
            new Tag { Id = Guid.NewGuid().ToString(), Name = "API", CreatedById = admin.Id, CreatedDateUtc = DateTime.UtcNow, UseCount = 0 },
        };

        foreach (var tag in tags)
        {
            await tagsCollection.CreateItemAsync(tag);
        }


        var tagsCsvFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "digital_bank_tags.csv");
        var csvTags = ReadTagsFromCsv(tagsCsvFilePath);

        foreach (var tag in csvTags)
        {
            tag.UseCount = 0;
            await tagsCollection.CreateItemAsync(tag);
        }

        #endregion

        #region Folders / CodeFiles
        
        string jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "public_file_system_nodes.json");
        var rootFolders = ReadFileSystemNodesFromJson(jsonFilePath);

        foreach (var rootFolder in rootFolders)
        {
            await ProcessNestedItemAsync(rootFolder);
        }

        #endregion 

        #region Public CodeAssets
        var codeAssetsCollection = await _dbContext.GetContainerAsync("CodeAssets");

        string codeAssetsJsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "public_code_assets.json");
        var codeAssets = ReadCodeAssetsFromJson(codeAssetsJsonFilePath);

        foreach (var codeAsset in codeAssets)
        {
            codeAsset.AssetType = AssetTypes.Public;
            codeAsset.CompanyId = null;
            await codeAssetsCollection.CreateItemAsync(codeAsset);
        }

        #endregion
    }

    public async Task InitializeCompaniesAsync()
    {
        var companiesCollection = await _dbContext.GetContainerAsync("Companies");
        var usersCollection = await _dbContext.GetContainerAsync("Users");
        var admin = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                .Where(u => u.Id == "652c3b89ae02a3135d6408fc")
                .AsEnumerable()
                .FirstOrDefault();

        var enterpriseUser = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                .Where(u => u.Id == "679e7985716a8576abe04445")
                .AsEnumerable()
                .FirstOrDefault();

        var companies = new List<Company>
        {
            new Company
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Tech Corp",
                Description = "A leading tech company.",
                CreatedById = admin.Id,
                CreatedDateUtc = DateTime.UtcNow
            },
            new Company
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Innovative Solutions",
                Description = "Provides cutting-edge solutions for AI and machine learning.",
                CreatedById = admin.Id,
                CreatedDateUtc = DateTime.UtcNow
            },
            new Company
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Green Energy Inc.",
                Description = "Focused on renewable energy solutions.",
                CreatedById = admin.Id,
                CreatedDateUtc = DateTime.UtcNow
            }
        };

        foreach (var company in companies)
        {
            await companiesCollection.CreateItemAsync(company);
        }

        enterpriseUser.CompanyId = companies[0].Id;
        await usersCollection.ReplaceItemAsync(enterpriseUser, enterpriseUser.Id);
    }

    // TODO: Add more Assets to code assets code_assets_integration_tests.json
    public async Task InitializeCodeAssetsForStartProjectsAsync()
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
            Name = "Start Project Demo",
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

        #region Folders / CodeFiles
        
        string jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "enterprise_file_system_nodes.json");
        var rootFolders = ReadFileSystemNodesFromJson(jsonFilePath);

        foreach (var rootFolder in rootFolders)
        {
            await ProcessNestedItemAsync(rootFolder);
        }

        #endregion 

        #region CodeAssets
        var codeAssetsCollection = await _dbContext.GetContainerAsync("CodeAssets");

        string codeAssetsJsonFilePath = Path.Combine(AppContext.BaseDirectory, "Static", "enterprise_code_assets.json");
        var codeAssets = ReadCodeAssetsFromJson(codeAssetsJsonFilePath);

        foreach (var codeAsset in codeAssets)
        {
            codeAsset.CompanyId = digitalBank.Id;
            codeAsset.AssetType = AssetTypes.Corporate;
            await codeAssetsCollection.CreateItemAsync(codeAsset);
        }

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
}
