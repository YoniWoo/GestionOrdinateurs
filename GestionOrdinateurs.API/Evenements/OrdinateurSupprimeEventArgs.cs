namespace GestionOrdinateurs.API.Evenements;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public int Id { get; init; }
    public string Nom { get; init; } = "";
}