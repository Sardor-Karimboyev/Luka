using Luka.Auth;
using Microsoft.Extensions.Primitives;
using StackExchange.Redis;

namespace Lukachi.Services.Users.Services;

public class RedisAccessTokenService : IAccessTokenService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TimeSpan _expires;

    public RedisAccessTokenService(IConnectionMultiplexer redis,
        IHttpContextAccessor httpContextAccessor,
        JwtOptions jwtOptions)
    {
        _redis = redis;
        _httpContextAccessor = httpContextAccessor;
        _expires = jwtOptions.Expiry ?? TimeSpan.FromMinutes(jwtOptions.ExpiryMinutes);
    }

    public Task<bool> IsCurrentActiveToken()
        => IsActiveAsync(GetCurrentToken());

    public Task DeactivateCurrentAsync()
        => DeactivateAsync(GetCurrentToken());

    public async Task<bool> IsActiveAsync(string token)
    {
        var db = _redis.GetDatabase();
        var key = GetKey(token);
        return !await db.KeyExistsAsync(key);
    }

    public async Task DeactivateAsync(string token)
    {
        var db = _redis.GetDatabase();
        var key = GetKey(token);
        await db.StringSetAsync(key, "revoked", _expires);
    }

    private string GetCurrentToken()
    {
        var header = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"];
        return string.IsNullOrWhiteSpace(header.ToString())
            ? string.Empty
            : header.ToString().Split(' ').Last();}

    private static string GetKey(string token) => $"blacklisted-tokens:{token}";
}