using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Luka.Types;

namespace Lukachi.Services.Users.Entities;

[Table("users", Schema = "identity")]
public class User : IIdentifiable<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Username { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedAt { get;  set; }
    public IEnumerable<UserRole> UserRoles { get; set; }
}