using Microsoft.AspNetCore.Http;

namespace Luka.Auth.Middlewares;

public class AccessTokenRevocationMiddleware
{
    private readonly RequestDelegate _next;

    public AccessTokenRevocationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAccessTokenService accessTokenService)
    {
        var isActive = await accessTokenService.IsCurrentActiveToken();
        if (!isActive)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token has been revoked.");
            return;
        }

        await _next(context);
    }
}