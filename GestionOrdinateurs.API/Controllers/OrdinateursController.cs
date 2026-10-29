using Microsoft.AspNetCore.Mvc;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.API.Evenements;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Authorization;


namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("api/ordinateurs")]
[Authorize]
public class OrdinateursController : ControllerBase
{
    private readonly IOrdinateurService _service;
    private readonly ILogger<OrdinateursController> _logger;
    private readonly IConfiguration _configuration;

    public OrdinateursController(
        IOrdinateurService service,
        ILogger<OrdinateursController> logger,
        IConfiguration configuration)
    {
        _service = service;
        _logger = logger;
        _configuration = configuration;

        _service.OrdinateurAjoute += AfficherMessageAjout;
        _service.OrdinateurMisAJour += AfficherMessageMisAJour;
        _service.OrdinateurSupprime += AfficherMessageSupprime;
    }

    [HttpGet]
    public async Task<IActionResult> Lister()
    {
        _logger.LogInformation("Récupération de la liste des ordinateurs");
        var ordinateur = await _service.ListerAsync();
        return Ok(ordinateur);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenirParId(int id)
    {
        _logger.LogInformation("Recherche de l'ordinateur avec l'identifiant {Id}", id);
        var ordinateur = await _service.ObtenirParIdAsync(id);

        if (ordinateur is null)
        {
            return NotFound();
        }

        return Ok(ordinateur);
    }

    [HttpPost]
    public async Task<IActionResult> Ajouter([FromBody] OrdinateurRequete requete)
    {
        var ordinateur = await _service.AjouterAsync(
            requete.Nom,
            requete.Marque,
            requete.RAM,
            requete.Processeur,
            requete.Etat,
            requete.Stock,
            requete.Prix,
            requete.StockageGo,
            requete.TypeOrdi);
        
        _logger.LogInformation(
            "Ordinateur ajouté.");

        return CreatedAtAction(
            nameof(ObtenirParId),
            new { id = ordinateur.Id },
            ordinateur);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Modifier(
        int id,
        [FromBody] OrdinateurRequete requete)
    {
        var modificationEffectuee = await _service.ModifierAsync(
            id,
            requete.Nom,
            requete.Marque,
            requete.RAM,
            requete.Processeur,    
            requete.Etat,
            requete.Stock,
            requete.Prix,
            requete.StockageGo,
            requete.TypeOrdi);

        if(!modificationEffectuee)
        {
            _logger.LogWarning(
                "Ordinateur introuvable.");
            return NotFound();
        }

        _logger.LogInformation(
            "Ordinateur modifié.");

        var ordinateur = await _service.ObtenirParIdAsync(id);
        return Ok(ordinateur);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Supprimer(int id)
    {
        var suppressionEffectuee = await _service.SupprimerAsync(id);
        
        if(!suppressionEffectuee)
        {
            _logger.LogWarning(
                "Ordinateur introuvable.");

            return NotFound();
        }

        _logger.LogInformation(
            "Ordinateur supprimé.");

        return NoContent();
    }

    [HttpGet("marque/{marque}")]
    public async Task<IActionResult> RechercherParMarque(string marque)
    {
        _logger.LogInformation("Recherche des ordinateurs de marque {Marque}", marque);
        var ordinateur = await _service.RechercheParMarqueAsync(marque);
        return Ok(ordinateur);
    }

    [HttpGet("info")]
    public IActionResult ObtenirInfoApplication()
    {
        _logger.LogInformation("Récupération des informations de l'application");
        var infomations = new
        {
            Nom = _configuration["Application:Name"],
            Version = _configuration["Application:Version"]
        };
        return Ok(infomations);
    }

    private void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        _logger.LogInformation(
            "Ordinateur ajouté : {Nom} (ID: {Id})", e.Nom, e.Id);
    }

    private void AfficherMessageMisAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        _logger.LogInformation(
            "Ordinateur mis à jour : {Nom} (ID: {Id})", e.Nom, e.Id);
    }

    private void AfficherMessageSupprime(object? sender, OrdinateurSupprimeEventArgs e)
    {
        _logger.LogInformation(
            "Ordinateur supprimé : {Nom} (ID: {Id})", e.Nom, e.Id);
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> ListerDisponibles()
    {
        _logger.LogInformation(
            "Consultation des ordinateurs disponibles."
        );

        var ordinateur = await _service.ListerDisponiblesAsync();

        return Ok(ordinateur);
    }

    [HttpGet("prix-decroissant")]
    public async Task<IActionResult> ListerParPrixDecroissant()
    {
        _logger.LogInformation(
            "Consultation des ordinateurs par prix décroissant."
        );
        var ordinateur = await _service.ListerParPrixDecroissantAsync();
        return Ok(ordinateur);
    }

    [HttpGet("{id:int}/diagnostic")]
    public async Task<IActionResult> ObtenirDiagnostic(int id)
    {
        _logger.LogInformation(
            "Consultation du diagnostic de l'ordinateur avec l'identifiant {Id}", id
        );

        var ordinateur = await _service.ObtenirParIdAsync(id);

        if (ordinateur is null)
        {
            return NotFound();
        }

        var diagnostic = _service.Diagnostiquer(ordinateur);

        return Ok(new
        { 
            OrdinateurId = ordinateur.Id,
            Diagnostic = diagnostic
        });
    }
    [HttpGet("statistiques")]
    public IActionResult ObtenirStatistiques()
    {
        _logger.LogInformation(
            "Consultation des statistiques du parc informatique."
        );

        var (total, enStock, valeurStock, nbReparation) =
         _service.CalculerStatistiques();

        return Ok(new
        {
            TotalOrdinateur = total,
            OrdiDisponibles = enStock,
            ValeurStock = valeurStock,
            NbReparations = nbReparation
        });
    }
    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> Chercher(string nom)
    {
        _logger.LogInformation(
            "Recherche dans l'inventaire par nom: {Nom}.", nom);

            var inventaire = new InventaireOrdinateurs
            {
                Ordinateurs = await _service.ListerAsync()
            };

            var ordinateur = inventaire[nom];

            if (ordinateur == null)
        {
            return NotFound();
        }

        return Ok(ordinateur);
    }
}