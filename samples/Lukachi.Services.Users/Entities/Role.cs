using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Luka.Types;

namespace Lukachi.Services.Users.Entities;

[Table("roles", Schema = "identity")]
public class Role : IIdentifiable<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<UserRole> UserRoles { get; set; }
    public IEnumerable<RolePermission> RolePermissions { get; set; }
}