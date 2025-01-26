using System.Security.Claims;
using AssetsManagerApi.Application.Models.Global;

namespace AssetsManagerApi.Api.Middlewares;

public class GlobalUserCustomMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        GlobalUser.Id = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;;
        GlobalUser.Name = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
        GlobalUser.Email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        GlobalUser.CompanyId = httpContext.User.FindFirst(ClaimTypes.GroupSid)?.Value;
        
        GlobalUser.Roles = [];
        foreach (var role in httpContext.User.FindAll(ClaimTypes.Role))
        {
            GlobalUser.Roles.Add(role.Value);
        }

        await this._next(httpContext);
    }
}
