namespace Lukachi.Services.Users.Commands;

public class CreateUser
{
    public string Username { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
}