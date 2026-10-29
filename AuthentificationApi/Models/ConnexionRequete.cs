using System.ComponentModel.DataAnnotations;

namespace AuthentificationApi.Models;

public class ConnexionRequete
{
    [Required]
    public string Login { get; set; } = "";
}