using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using GestionOrdinateurs.API.Data;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared;
using Xunit;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest : IDisposable
{
    private ParcInformatiqueContext CreerContexte()
    {
        var options =
            new DbContextOptionsBuilder<ParcInformatiqueContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ParcInformatiqueContext(options);
    }

    private readonly string _dossierTest = Path.Combine(
        Path.GetTempPath(),
        "GestionOrdinateursTests",
        Guid.NewGuid().ToString());

    private JournalService CreerJournal()
    {
        Directory.CreateDirectory(_dossierTest);

        var builder = WebApplication.CreateBuilder(
            new WebApplicationOptions
            {
                ContentRootPath = _dossierTest
            });

        return new JournalService(builder.Environment);    
    }

    public void Dispose()
    {
        if (Directory.Exists(_dossierTest))
        {
            Directory.Delete(_dossierTest, recursive: true);
        }
    }

    [Fact]
    public async Task Ajouter_InformationsValides_AjouteOrdinateurAuParc()
    {
        using var contexte = CreerContexte();
        var journal = CreerJournal();
        var service = new OrdinateurService(contexte, journal);

        var ordinateur = await service.AjouterAsync(
            "PC-Test",
            "MSI",
            16,
            "AMD",
            EtatOrdinateur.EnService,
            3,
            900,
            512,
            TypeOrdinateur.Portable);
        
        contexte.ChangeTracker.Clear();

        var ordinateurEnregistre = await service.ObtenirParIdAsync(ordinateur.Id);

        var ordinateurs = await service.ListerAsync();

        ordinateurs.Should().ContainSingle();
        ordinateurEnregistre.Should().NotBeNull();
        Assert.NotNull(ordinateurEnregistre);

        ordinateurEnregistre!.Id.Should().BeGreaterThan(0);
        ordinateurEnregistre!.Nom.Should().Be("PC-Test");
        ordinateurEnregistre!.Caracteristiques.Marque.Should().Be("MSI");
        ordinateurEnregistre!.Caracteristiques.RamGo.Should().Be(16);
        ordinateurEnregistre!.Caracteristiques.Processeur.Should().Be("AMD");
        ordinateurEnregistre!.Etat.Should().Be(EtatOrdinateur.EnService);
        ordinateurEnregistre!.Stock.Should().Be(3);
        ordinateurEnregistre!.Prix.Should().Be(900);
        ordinateurEnregistre!.Caracteristiques.StockageGo.Should().Be(512);
        ordinateurEnregistre!.Caracteristiques.TypeOrdi.Should().Be(TypeOrdinateur.Portable);
    }

    [Fact]
    public async Task Supprimer_OrdinateurExistant_RetireOrdinateurDuParc()
    {
        using var contexte = CreerContexte();
        var journal = CreerJournal();
        var service = new OrdinateurService(contexte, journal);

         var ordinateur = await service.AjouterAsync(
            "PC-Test",
            "MSI",
            16,
            "AMD",
            EtatOrdinateur.EnService,
            3,
            900,
            512,
            TypeOrdinateur.Portable);

        var resultat = await service.SupprimerAsync(ordinateur.Id);

        contexte.ChangeTracker.Clear();

        var ordinateurSupprime = await service.ObtenirParIdAsync(ordinateur.Id);
        var ordinateurs = await service.ListerAsync();

        resultat.Should().BeTrue();
        ordinateurSupprime.Should().BeNull();
        ordinateurs.Should().BeEmpty();
    }

    [Fact]
    public async Task Supprimer_OrdinateurInexistant_RetourneFalse()
    {
        using var contexte = CreerContexte();
        var journal = CreerJournal();
        var service = new OrdinateurService(contexte, journal);

         var ordinateur = await service.AjouterAsync(
            "PC-Test",
            "MSI",
            16,
            "AMD",
            EtatOrdinateur.EnService,
            3,
            900,
            512,
            TypeOrdinateur.Portable);

        var resultat = await service.SupprimerAsync(ordinateur.Id + 1);

        contexte.ChangeTracker.Clear();

        var ordinateurConserve = await service.ObtenirParIdAsync(ordinateur.Id);
        var ordinateurs = await service.ListerAsync();

        resultat.Should().BeFalse();
        ordinateurs.Should().ContainSingle();

        Assert.NotNull(ordinateurConserve);
        ordinateurConserve.Id.Should().Be(ordinateur.Id);
        ordinateurConserve.Nom.Should().Be("PC-Test");
        ordinateurConserve.Stock.Should().Be(3);
        ordinateurConserve.Prix.Should().Be(900);
    }
    [Fact]
    public async Task CreerReparation_InformationValides_EnregistreReparation()
    {
        using var contexte = CreerContexte();

        var ordinateur = new Ordinateur();

        ordinateur.ChangerNom("PC-Test-Reparation");
        ordinateur.ChangerRAM(16);
        ordinateur.ChangerMarque("Dell");
        ordinateur.ChangerProcesseur("Intel Core i5");
        ordinateur.ChangerStockage(512);
        ordinateur.ChangerTypeOrdinateur(TypeOrdinateur.Portable);
        ordinateur.ChangerEtat(EtatOrdinateur.EnPanne);

        contexte.Ordinateurs.Add(ordinateur);
        await contexte.SaveChangesAsync();

        var dateDebut = DateTime.Now;

        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            UnOrdinateur = ordinateur,
            Technicien = "Technicien de test",
            TypeReparation = "Remplacement SSD",
            DateDebut = dateDebut,
            Statut = "En cours"
        };

        contexte.Reparations.Add(reparation);
        await contexte.SaveChangesAsync();

        contexte.ChangeTracker.Clear();

        var reparationEnregistree = await contexte.Reparations
            .Include(element => element.UnOrdinateur)
            .SingleAsync();

        reparationEnregistree.Id.Should().BeGreaterThan(0);
        reparationEnregistree.OrdinateurId.Should().Be(ordinateur.Id);
        reparationEnregistree.UnOrdinateur.Nom
            .Should().Be("PC-Test-Reparation");
        reparationEnregistree.Technicien.Should().Be("Technicien de test");
        reparationEnregistree.TypeReparation.Should().Be("Remplacement SSD");
        reparationEnregistree.DateDebut.Should().Be(dateDebut);
        reparationEnregistree.Statut.Should().Be("En cours");
    }
}