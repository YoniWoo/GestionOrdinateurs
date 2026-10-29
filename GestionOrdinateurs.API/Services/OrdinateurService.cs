using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.API.Evenements;

namespace GestionOrdinateurs.API.Services;

public class OrdinateurService : IOrdinateurService
{
    private readonly ParcInformatiqueContext _context;
    private readonly JournalService _journalService;

    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;

    public OrdinateurService(
        ParcInformatiqueContext context,
        JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
    }

    public async Task<List<Ordinateur>> ListerAsync()
    {
        return await _context.Ordinateurs.ToListAsync();
    }

    public async Task<Ordinateur?> ObtenirParIdAsync(int id)
    {
        return await _context.Ordinateurs
            .FirstOrDefaultAsync(ordinateur => ordinateur.Id == id);
    }

    public async Task<Ordinateur> AjouterAsync(
        string nom,
        string marque,
        int ram,
        string processeur,
        EtatOrdinateur etat,
        int stock,
        int prix,
        int stockageGo,
        TypeOrdinateur typeOrdi)
    {
        VerifierInformations(nom, marque, ram, processeur, etat);

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Le stock ne peut pas être négatif.");
        }
        if (prix < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(prix),
                "Le prix ne peut pas être négatif.");
        }
        if (stockageGo <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stockageGo),
                "La capacité de stockage ne peut pas être négative.");
        }

        var ordinateur = new Ordinateur();
        ordinateur.ChangerNom(nom);
        ordinateur.ChangerMarque(marque);
        ordinateur.ChangerRAM(ram);
        ordinateur.ChangerProcesseur(processeur);
        ordinateur.ChangerEtat(etat);
        ordinateur.ChangerStock(stock);
        ordinateur.ChangerPrix(prix);
        ordinateur.ChangerStockage(stockageGo);
        ordinateur.ChangerTypeOrdinateur(typeOrdi);

        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();

        _journalService.EcrireOrdinateur(
            $"Ordinateur ajouté: {ordinateur.Nom} (ID: {ordinateur.Id})"
        );

        OrdinateurAjoute?.Invoke(
            this,
            new OrdinateurAjouteEventArgs
            {
                Id = ordinateur.Id,
                Nom = ordinateur.Nom
            }
        );

        return ordinateur;
    }
    
    public async Task<bool> ModifierAsync(
        int id,
        string nom,
        string marque,
        int ram,
        string processeur,
        EtatOrdinateur etat,
        int stock,
        int prix,
        int stockageGo,
        TypeOrdinateur typeOrdi)
    {
        var ordinateur = await ObtenirParIdAsync(id);

        if (ordinateur is null)
        {
            return false;
        }
        VerifierInformations(nom, marque, ram, processeur, etat);

        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stock),
                "Le stock ne peut pas être négatif.");
        }
        if (prix < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(prix),
                "Le prix ne peut pas être négatif.");
        }
        if (stockageGo <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stockageGo),
                "La capacité de stockage ne peut pas être négative.");
        }
        if (!Enum.IsDefined(typeof(TypeOrdinateur), typeOrdi))
        {
            throw new ArgumentException(
                "Le type d'ordinateur spécifié n'est pas valide.", nameof(typeOrdi));
        }

        ordinateur.ChangerNom(nom);
        ordinateur.ChangerMarque(marque);
        ordinateur.ChangerRAM(ram);
        ordinateur.ChangerProcesseur(processeur);
        ordinateur.ChangerEtat(etat);
        ordinateur.ChangerStock(stock);
        ordinateur.ChangerPrix(prix);
        ordinateur.ChangerStockage(stockageGo);
        ordinateur.ChangerTypeOrdinateur(typeOrdi);

        await _context.SaveChangesAsync();
        _journalService.EcrireOrdinateur(
            $"Ordinateur modifié: {ordinateur.Nom} (ID: {ordinateur.Id})"
        );

        OrdinateurMisAJour?.Invoke(
            this,
            new OrdinateurMisAJourEventArgs
            {
                Id = ordinateur.Id,
                Nom = ordinateur.Nom
            }
        );

        return true;
    }
    
    public async Task<bool> SupprimerAsync(int id)
    {
        var ordinateur = await ObtenirParIdAsync(id);

        if (ordinateur is null)
        {
            return false;
        }

        _context.Ordinateurs.Remove(ordinateur);
        await _context.SaveChangesAsync();
        _journalService.EcrireOrdinateur(
            $"Ordinateur supprimé: {ordinateur.Nom} (ID: {ordinateur.Id})"
        );

        OrdinateurSupprime?.Invoke(
            this,
            new OrdinateurSupprimeEventArgs
            {
                Id = ordinateur.Id,
                Nom = ordinateur.Nom
            }
        );

        return true;
    }

    private static void VerifierInformations(
        string nom,
        string marque,
        int ram,
        string processeur,
        EtatOrdinateur etat)
    {
        if (nom is null || nom.Trim() == "")
        {
            throw new ArgumentException(
                "Le nom ne peut pas être vide.", nameof(nom));
        }

        if (marque is null || marque.Trim() == "")
        {
            throw new ArgumentException(
                "La marque ne peut pas être vide.", nameof(marque));
        }

        if (ram <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ram),
                "La quantité de RAM doit être supérieure à zéro.");
        }

        if (processeur is null || processeur.Trim() == "")
        {
            throw new ArgumentException(
                "Le processeur ne peut pas être vide.", nameof(processeur));
        }

        if (!Enum.IsDefined(typeof(EtatOrdinateur), etat))
        {
            throw new ArgumentOutOfRangeException(
                nameof(etat),
                "L'état de l'ordinateur n'est pas valide.");
        }
    }

    public async Task<List<Ordinateur>> RechercheParMarqueAsync(string marque)
    {
        var rechercheMarque = marque?.Trim().ToLower();

        return await _context.Ordinateurs
            .Where(ordinateur =>
                ordinateur.Caracteristiques.Marque.ToLower() == rechercheMarque)
            .ToListAsync();
    }

    public async Task<List<Ordinateur>> ListerDisponiblesAsync()
    {
        return await _context.Ordinateurs
            .Where(ordinateur => ordinateur.Stock > 0)
            .ToListAsync();
    }

    public async Task<List<Ordinateur>> ListerParPrixDecroissantAsync()
    {
        var ordinateur = await _context.Ordinateurs.ToListAsync();

        return ordinateur
        .OrderByDescending(o => o.Prix).ToList();
    }

    public string Diagnostiquer(Ordinateur ordinateur)
    {
        return ordinateur switch
        {
            {
                Etat: EtatOrdinateur.HorsService
            }
                => "Remplacement",

            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques:
                {
                    RamGo: < 8
                }
            }
                => "Ajouter RAM",

            _
                => "Aucune action"
        };
    }

    public (
        int TotalOrdinateur,
        int OrdiDisponibles,
        decimal ValeurStock,
        int NbReparations) CalculerStatistiques()
    {
        var ordinateurs = _context.Ordinateurs.ToList();
        var totalOrdinateur = ordinateurs.Count;
        var ordinateurDisponible = ordinateurs.Count(
            ordinateur => ordinateur.Stock > 0);
        var valeurStock = ordinateurs.Sum(
            ordinateur => (decimal)ordinateur.Prix * ordinateur.Stock);
        var nbReparations = _context.Reparations.Count();

        return (
            totalOrdinateur,
            ordinateurDisponible,
            valeurStock,
            nbReparations);
    }
}