namespace GestionOrdinateurs.Client.Models;

public class HistoriqueReparationResponse
{
    public int OrdinateurId { get; set; }

    public string NomOrdinateur { get; set; } = "";

    public string Technicien { get; set; } = "";

    public string TypeReparation { get; set; } = "";

    public DateTime DateIntervention { get; set; }
}