using Microsoft.IdentityModel.JsonWebTokens;

namespace Luka.Auth;

public interface IJwtHandler
{
    JsonWebToken CreateToken(string userId, string role = null, string audience = null,
        IDictionary<string, IEnumerable<string>> claims = null);

    JsonWebTokenPayload GetTokenPayload(string accessToken);
}