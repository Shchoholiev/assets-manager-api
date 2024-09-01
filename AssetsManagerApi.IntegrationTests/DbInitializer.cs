using AssetsManagerApi.Domain.Entities.Identity;
using AssetsManagerApi.Infrastructure.Services.Identity;
using AssetsManagerApi.Persistance.Db;
using Microsoft.Extensions.Logging;

namespace AssetsManagerApi.IntegrationTests;

public class DbInitializer(CosmosDbContext dbContext)
{
    private readonly CosmosDbContext _dbContext = dbContext;

    public void InitializeDb()
    {
        CleanDatabase().Wait();
        
        InitializeUsersAsync().Wait();
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
