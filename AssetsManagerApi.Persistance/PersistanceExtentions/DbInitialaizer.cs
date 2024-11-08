using AssetsManagerApi.Domain.Entities;
using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Domain.Enums;
using AssetsManagerApi.Infrastructure.Services.Identity;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Extensions.Logging;

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
    }

    public async Task InitializeCodeAssetsAsync()
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

        var user1 = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                                   .Where(u => u.Id == "652c3b89ae02a3135d6439fc")
                                   .AsEnumerable()
                                   .FirstOrDefault();

        var user2 = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                                   .Where(u => u.Id == "652c3b89ae02a3135d6432fc")
                                   .AsEnumerable()
                                   .FirstOrDefault();

        var cSharpTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "CSharp",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(cSharpTag);

        var javaTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Java",
            CreatedById = user1.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(javaTag);

        var azureTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Azure",
            CreatedById = user2.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(azureTag);

        var apiTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "API",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(apiTag);

        var consoleAppTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "ConsoleApp",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(consoleAppTag);

        var scriptingTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Scripting",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(scriptingTag);

        var utilityTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Utility",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(utilityTag);

        var pythonTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Python",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(pythonTag);

        var javaScriptTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "JavaScript",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(javaScriptTag);

        var webDevelopmentTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "WebDevelopment",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(webDevelopmentTag);

        var functionsTag = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Functions",
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            UseCount = 0
        };

        await tagsCollection.CreateItemAsync(functionsTag);

        var codeAssets = new List<CodeAsset>();

        var folder1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "CSharpProjectFolder",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(folder1);

        var codeFile1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Program.cs",
            ParentId = folder1.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
using System;

namespace HelloWorldApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
            Console.WriteLine(""This is a C# project demonstrating basic syntax."");
        }
    }
}",
            Language = Languages.csharp,
        };
        await codeFilesCollection.CreateItemAsync(codeFile1);

        var codeAsset1 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "CSharp Project",
            Description = "A simple C# project demonstrating basic syntax.",
            RootFolderId = folder1.Id,
            PrimaryCodeFile = codeFile1,
            AssetType = AssetTypes.Private,
            Language = Languages.csharp,
            Tags = new List<Tag>
            {
                cSharpTag, consoleAppTag
            },
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        codeAssets.Add(codeAsset1);

        var folder2 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "PythonProjectFolder",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(folder2);

        var codeFile2 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "script.py",
            ParentId = folder2.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
def greet(name):
    print(f'Hello, {name}! Welcome to the Python project.')

def add_numbers(a, b):
    return a + b

if __name__ == '__main__':
    greet('User')
    result = add_numbers(5, 10)
    print(f'The sum of 5 and 10 is {result}')
",
            Language = Languages.python,
        };
        await codeFilesCollection.CreateItemAsync(codeFile2);

        var codeAsset2 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Python Script",
            Description = "A Python script with a basic greeting and addition function.",
            RootFolderId = folder2.Id,
            PrimaryCodeFile = codeFile2,
            AssetType = AssetTypes.Public,
            Language = Languages.python,
            Tags = new List<Tag>
            {
                pythonTag, scriptingTag, utilityTag
            },
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        codeAssets.Add(codeAsset2);

        var folder3 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "JavaScriptProjectFolder",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(folder3);

        var subFolder3_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Utils",
            ParentId = folder3.Id,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(subFolder3_1);

        var codeFile3_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "app.js",
            ParentId = folder3.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
function greet(name) {
    console.log(`Hello, ${name}!`);
}

function multiply(a, b) {
    return a * b;
}

greet('Developer');
const result = multiply(6, 7);
console.log(`The product of 6 and 7 is ${result}`);
",
            Language = Languages.javascript,
        };
        await codeFilesCollection.CreateItemAsync(codeFile3_1);

        var codeFile3_2 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "mathUtils.js",
            ParentId = subFolder3_1.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
export function add(a, b) {
    return a + b;
}

export function subtract(a, b) {
    return a - b;
}",
            Language = Languages.javascript,
        };
        await codeFilesCollection.CreateItemAsync(codeFile3_2);

        var codeAsset3 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "JavaScript Project",
            Description = "A JavaScript project with multiple utility functions.",
            RootFolderId = folder3.Id,
            PrimaryCodeFile = codeFile3_1,
            AssetType = AssetTypes.Corporate,
            Language = Languages.javascript,
            Tags = new List<Tag>
            {
                javaScriptTag, functionsTag, utilityTag, webDevelopmentTag
            },
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        codeAssets.Add(codeAsset3);

        var folder4 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AnotherCSharpProjectFolder",
            ParentId = null,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(folder4);

        var subFolder4_1 = new Folder
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Services",
            ParentId = folder4.Id,
            Type = FileType.Folder,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        await foldersCollection.CreateItemAsync(subFolder4_1);

        var codeFile4_1 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "AnotherProgram.cs",
            ParentId = folder4.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
using System;

namespace AnotherCSharpApp
{
    class AnotherProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello from another C# project!"");

            int result = AddNumbers(10, 20);
            Console.WriteLine($""The sum of 10 and 20 is {result}"");
        }

        static int AddNumbers(int a, int b)
        {
            return a + b;
        }
    }
}",
            Language = Languages.csharp,
        };
        await codeFilesCollection.CreateItemAsync(codeFile4_1);

        var codeFile4_2 = new CodeFile
        {
            Id = Guid.NewGuid().ToString(),
            Name = "MathService.cs",
            ParentId = subFolder4_1.Id,
            Type = FileType.CodeFile,
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow,
            Text = @"
using System;

namespace AnotherCSharpApp.Services
{
    public class MathService
    {
        public int Multiply(int a, int b)
        {
            return a * b;
        }
    }
}",
            Language = Languages.csharp,
        };
        await codeFilesCollection.CreateItemAsync(codeFile4_2);

        var codeAsset4 = new CodeAsset
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Another CSharp Project",
            Description = "Another C# project with a service and utility methods.",
            RootFolderId = folder4.Id,
            PrimaryCodeFile = codeFile4_1,
            AssetType = AssetTypes.Private,
            Language = Languages.csharp,
            Tags = new List<Tag>
            {
                cSharpTag, utilityTag, consoleAppTag
            },
            CreatedById = admin.Id,
            CreatedDateUtc = DateTime.UtcNow
        };
        codeAssets.Add(codeAsset4);

        cSharpTag.UseCount = 2;
        consoleAppTag.UseCount = 2;
        scriptingTag.UseCount = 1;
        utilityTag.UseCount = 3;
        pythonTag.UseCount = 1; 
        javaScriptTag.UseCount = 1;
        webDevelopmentTag.UseCount = 1;
        functionsTag.UseCount = 1;

        await tagsCollection.ReplaceItemAsync(cSharpTag, cSharpTag.Id);
        await tagsCollection.ReplaceItemAsync(consoleAppTag, consoleAppTag.Id);
        await tagsCollection.ReplaceItemAsync(scriptingTag, scriptingTag.Id);
        await tagsCollection.ReplaceItemAsync(utilityTag, utilityTag.Id);
        await tagsCollection.ReplaceItemAsync(pythonTag, pythonTag.Id);
        await tagsCollection.ReplaceItemAsync(javaScriptTag, javaScriptTag.Id);
        await tagsCollection.ReplaceItemAsync(webDevelopmentTag, webDevelopmentTag.Id);
        await tagsCollection.ReplaceItemAsync(functionsTag, functionsTag.Id);

        foreach (var codeAsset in codeAssets)
        {
            await codeAssetsCollection.CreateItemAsync(codeAsset);
        }
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

        #endregion

        #region Users

        var passwordHasher = new PasswordHasher(new Logger<PasswordHasher>(new LoggerFactory()));
        var usersCollection = await _dbContext.GetContainerAsync("Users");

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

    public async Task InitializeCompaniesAsync()
    {
        var companiesCollection = await _dbContext.GetContainerAsync("Companies");
        var usersCollection = await _dbContext.GetContainerAsync("Users");
        var admin = usersCollection.GetItemLinqQueryable<User>(allowSynchronousQueryExecution: true)
                .Where(u => u.Id == "652c3b89ae02a3135d6408fc")
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
