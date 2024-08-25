using AssetsManagerApi.Api.Middlewares;
using AssetsManagerApi.Application;
using AssetsManagerApi.Infrastructure;
using AssetsManagerApi.Persistance;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

app.Run();

public partial class Program {}