using Luka.Attributes;

namespace Lukachi.Services.Users.Auth;

[Permission]
public enum Permissions
{
    CREATE_USER,
    GET_USER,
    BROWSE_USERS
}