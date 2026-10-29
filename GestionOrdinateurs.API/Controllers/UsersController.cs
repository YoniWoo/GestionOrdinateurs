using GestionOrdinateurs.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _context;
    private readonly  ILogger<UsersController> _logger;

    public UsersController(
        ParcInformatiqueContext context,
        ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Lister()
    {
        _logger.LogInformation(
            "Consultation de la liste des utilisateurs."
        );
        var utilisateurs = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return Ok(utilisateurs);
    }
    [HttpGet("login/{login}")]
    public async Task<IActionResult> ObtenirParLogin(string login)
    {
        _logger.LogInformation(
            "Recherche de l'utilisateur {login}.", login
        );

        var utilisateur = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                utilisateur => utilisateur.Login == login);
        if (utilisateur == null)
        {
            return NotFound();
        }
        return Ok(utilisateur);
    }
}