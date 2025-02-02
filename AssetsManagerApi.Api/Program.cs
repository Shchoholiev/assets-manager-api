using System.Reflection;
using AssetsManagerApi.Api.Middlewares;
using AssetsManagerApi.Application;
using AssetsManagerApi.Infrastructure;
using AssetsManagerApi.Persistance;
using AssetsManagerApi.Persistance.Db;
using AssetsManagerApi.Persistance.PersistanceExtentions;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Configuration.AddJsonFile($"appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddMapper();
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);
builder.Services.AddJWTTokenAuthentication(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();

builder.Services
    .AddCors(options =>
        {
            options.AddPolicy("allowAnyOrigin",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("allowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalUserCustomMiddleware>();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program {}