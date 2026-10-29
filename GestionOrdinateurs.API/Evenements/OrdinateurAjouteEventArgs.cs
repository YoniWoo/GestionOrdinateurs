namespace GestionOrdinateurs.API.Evenements;

public class OrdinateurAjouteEventArgs : EventArgs
{
    public int Id { get; init; }
    public string Nom { get; init; } = "";
}