namespace GestionOrdinateurs.Client.Models;

public class OrdinateurListeResponse
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public int Prix { get; set; }
    public bool Vendu { get; set; }
}