using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Client.Models;

public class ConnexionRequete
{
    [Required(ErrorMessage = "Veuillez saisir un login.")]
    public string Login { get; set; } = "";
}