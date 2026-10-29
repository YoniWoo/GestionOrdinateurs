namespace GestionOrdinateurs.Client.Models;

public class ConnexionResponse
{
    public string Token { get; set; } = "";

    public DateTime Expiration { get; set; }
}