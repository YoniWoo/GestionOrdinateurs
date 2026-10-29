namespace GestionOrdinateurs.API.Evenements;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public int Id { get; init; }
    public string Nom { get; init; } = "";
}