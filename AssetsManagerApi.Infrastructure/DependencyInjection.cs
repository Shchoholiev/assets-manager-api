using System.Net.Http.Headers;
using System.Text;
using AssetsManagerApi.Application.IServices;
using AssetsManagerApi.Application.IServices.Identity;
using AssetsManagerApi.Infrastructure.Services;
using AssetsManagerApi.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AssetsManagerApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddScoped<IRolesService, RolesService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<ITokensService, TokensService>();
        services.AddScoped<IEmailsService, GmailSmtpEmailsService>();
        services.AddScoped<ICodeAssetsService, CodeAssetsService>();
        services.AddScoped<ITagsService, TagsService>();
        services.AddScoped<IFoldersService, FoldersService>();
        services.AddScoped<ICodeFilesService, CodeFilesService>();
        services.AddScoped<ICompaniesService, CompaniesService>();

        services.AddScoped<IStartProjectsService, StartProjectsService>();
        services.AddScoped<IGenerativeAiService, OpenAIService>();

        services.AddScoped<INugetService, NugetService>();

        services.AddHttpClient<IGenerativeAiService, OpenAIService>(client =>
        {
            var openAiBaseUrl = configuration.GetValue<string>("OpenAi:BaseUrl")!;
            client.BaseAddress = new Uri(openAiBaseUrl);
            
            var openAiApiKey = configuration.GetValue<string>("OpenAi:ApiKey")!;
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", openAiApiKey);
        });


        services.AddScoped<ICompilationService, CompilationService>();

        return services;
    }

    public static IServiceCollection AddJWTTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateIssuer"),
                    ValidateAudience = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateAudience"),
                    ValidateLifetime = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateLifetime"),
                    ValidateIssuerSigningKey = configuration.GetValue<bool>("JsonWebTokenKeys:ValidateIssuerSigningKey"),
                    ValidIssuer = configuration.GetValue<string>("JsonWebTokenKeys:ValidIssuer"),
                    ValidAudience = configuration.GetValue<string>("JsonWebTokenKeys:ValidAudience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JsonWebTokenKeys:IssuerSigningKey"))),
                    ClockSkew = TimeSpan.Zero
                };
            });
        services.AddAuthorization();

        return services;
    }
}
