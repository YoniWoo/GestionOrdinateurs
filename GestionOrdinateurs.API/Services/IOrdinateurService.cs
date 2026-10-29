using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Evenements;

namespace GestionOrdinateurs.API.Services;

public interface IOrdinateurService
{
    Task<List<Ordinateur>> ListerAsync();
    Task<Ordinateur?> ObtenirParIdAsync(int id);
    Task<List<Ordinateur>> RechercheParMarqueAsync(string marque);

    event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

    Task<Ordinateur> AjouterAsync(
        string nom,
        string marque,
        int ram,
        string processeur,
        EtatOrdinateur etat,
        int stock,
        int prix,
        int stockageGo,
        TypeOrdinateur typeOrdi
    );

    Task<bool> ModifierAsync(
        int id,
        string nom,
        string marque,
        int ram,
        string processeur,
        EtatOrdinateur etat,
        int stock,
        int prix,
        int stockageGo,
        TypeOrdinateur typeOrdi
    );

    Task<bool> SupprimerAsync(int id);
    
    Task<List<Ordinateur>> ListerDisponiblesAsync();

    Task<List<Ordinateur>> ListerParPrixDecroissantAsync();

    string Diagnostiquer(Ordinateur ordinateur);

    (int TotalOrdinateur, int OrdiDisponibles,
        decimal ValeurStock, int NbReparations) CalculerStatistiques();
}