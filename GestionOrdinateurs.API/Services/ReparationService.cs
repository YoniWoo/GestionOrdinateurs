using System.Text;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.Shared;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Services;

public class ReparationService
{
    private readonly IDbContextFactory<ParcInformatiqueContext> _fabriqueContexte;
    
    private readonly JournalService _journalService;

    public ReparationService(
        IDbContextFactory<ParcInformatiqueContext> fabriqueContexte,
        JournalService journalService)
    {
        _fabriqueContexte = fabriqueContexte;
        _journalService = journalService;
    }

    public record HistoriqueReparationRecord(
        int OrdinateurId,
        string NomOrdinateur,
        string Technicien,
        string TypeReparation,
        DateTime DateIntervention
    );

   public async Task<Reparation> AddAsync(
        ParcInformatiqueContext contexte,
        Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Réparation simulée",
            DateDebut = DateTime.Now,
            Statut = "En cours",
            Technicien = "Technicien de test"
        };

        contexte.Reparations.Add(reparation);
        await contexte.SaveChangesAsync();

        return reparation;
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        await using var contexte =
            await _fabriqueContexte.CreateDbContextAsync();
        var reparation = await AddAsync(contexte, ordinateur);
        await Task.Delay(2000);
        reparation.Statut = "Terminée";
        await contexte.SaveChangesAsync();
        _journalService.EcrireReparation($"{ordinateur.Nom} réparé");
        Console.WriteLine(
            $"{ordinateur.Nom} réparé");
    }

    public async Task<List<Reparation>> ListerAsync()
    {
        await using var contexte =
            await _fabriqueContexte.CreateDbContextAsync();
        return await contexte.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .ToListAsync();
    }

    public async Task<List<HistoriqueReparationRecord>> ObtenirHistoriqueReparationsAsync()
    {
        await using var contexte =
            await _fabriqueContexte.CreateDbContextAsync();

        return await contexte.Reparations
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.OrdinateurId,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut
            ))
            .ToListAsync();
    }
}