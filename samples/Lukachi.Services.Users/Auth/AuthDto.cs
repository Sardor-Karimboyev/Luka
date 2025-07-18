namespace Lukachi.Services.Users.Auth;

public class AuthDto
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string Role { get; set; }
    public long Expires { get; set; }
}