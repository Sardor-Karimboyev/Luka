using Luka.Auth;

namespace Lukachi.Services.Users.Auth;

public class JwtProvider : IJwtProvider
{
    private readonly IJwtHandler _jwtHandler;

    public JwtProvider(IJwtHandler jwtHandler)
    {
        _jwtHandler = jwtHandler;
    }

    public AuthDto Create(string userId, string role, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null)
    {
        var jwt = _jwtHandler.CreateToken(userId, role, audience, claims);

        return new AuthDto
        {
            AccessToken = jwt.AccessToken,
            Role = jwt.Role,
            Expires = jwt.Expires
        };
    }
}