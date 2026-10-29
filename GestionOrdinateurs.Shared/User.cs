using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;

public class User
{
    public int Id { get; set; }

    [Required]
    public string Login { get; set; } = "";
    public RoleEnum Role { get; set; }
}