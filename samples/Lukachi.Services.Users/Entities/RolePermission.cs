using System.ComponentModel.DataAnnotations.Schema;

namespace Lukachi.Services.Users.Entities;

[Table("role_permission", Schema = "identity")]
public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }
}