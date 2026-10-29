using GestionOrdinateurs.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("api/reparations")]
[Authorize]
public class ReparationController : ControllerBase
{
    private readonly IOrdinateurService _ordinateurService;
    private readonly ReparationService _reparService;  
    private readonly ILogger<ReparationController> _logger;
    private readonly StockService _stockService;

    public ReparationController(
        IOrdinateurService ordinateurService,
        ReparationService reparationService,
        ILogger<ReparationController> logger,
        StockService stockService)
    {
        _ordinateurService = ordinateurService;
        _reparService = reparationService;
        _logger = logger;
        _stockService = stockService;
    }

    [HttpPost("simuler")]
    public async Task<IActionResult> SimulerReparation()
    {
        var ordinateurs = await _ordinateurService.ListerAsync();

        var listOrdinateurs = ordinateurs
            .OrderBy(ordinateur => ordinateur.Id)
            .Take(5)
            .ToList();

        if (listOrdinateurs.Count < 5)
        {
            _logger.LogWarning(
                "Simulation de réparation annulée : moins de 5 ordinateurs disponibles."
            );

            return BadRequest("Moins de 5 ordinateurs disponibles pour la simulation.");
        }

        _logger.LogInformation("Simulation de réparation pour les 5 premiers ordinateurs.");

        await Task.WhenAll(
            listOrdinateurs.Select(
                ordinateur => _reparService.SimulerReparationAsync(ordinateur)
            )
        );

        return Ok("Simulation de réparation terminée pour les 5 premiers ordinateurs.");
    }

    [HttpGet]
    public async Task<IActionResult> Lister()
    {
        _logger.LogInformation(
            "Récupération de la liste des réparations");

        var reparations = await _reparService.ListerAsync();
        return Ok(reparations);
    }

    [HttpGet("GetStockSSD")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetStockSSD()
    {
        Parallel.For(
            0,
            10,
            i => _stockService.ConsommerSSD());
        
        _logger.LogInformation(
            "Stock de SSD après consommation : {StockSSD}", _stockService.StockSSD);

        return Ok(_stockService.StockSSD);
    }

    [HttpGet("historique")]
    public async Task<IActionResult> ObtenirHistorique()
    {
        _logger.LogInformation(
            "Récupération de l'historique des réparations");

        var historique = await _reparService.ObtenirHistoriqueReparationsAsync();
        return Ok(historique);
    }
}
