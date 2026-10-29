namespace GestionOrdinateurs.Shared;

public class InventaireOrdinateurs
{
    public List<Ordinateur> Ordinateurs { get; set; } = new();

    public Ordinateur? this[string reference]
    {
        get
        {
            return Ordinateurs.FirstOrDefault(
                ordinateur => string.Equals(
                    ordinateur.Nom,
                    reference,
                    StringComparison.OrdinalIgnoreCase
                ));
        }
    }

    public Ordinateur? this[int ordinateurId]
    {
        get
        {
            return Ordinateurs.FirstOrDefault(
                ordinateur => ordinateur.Id == ordinateurId);
        }
    }
}