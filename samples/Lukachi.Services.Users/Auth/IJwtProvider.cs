namespace Lukachi.Services.Users.Auth;

public interface IJwtProvider
{
    AuthDto Create(string userId, string role, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null);
}