using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using AuthentificationApi.Models;
using GestionOrdinateurs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthentificationApi.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AuthenticationController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Connecter(
        [FromBody] ConnexionRequete requete)
    {
        var login = requete.Login.Trim();

        using var client =
            _httpClientFactory.CreateClient("GestionOrdinateurs");

        User? utilisateur;

        try
        {
            var chemin =
                $"api/users/login/{Uri.EscapeDataString(login)}";

            using var reponse = await client.GetAsync(chemin);

            if (reponse.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(new
                {
                    Message = "Cet utilisateur n'existe pas."
                });
            }

            if (!reponse.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Recherche utilisateur refusée par l'API : {Statut}.",
                    reponse.StatusCode);

                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new { Message = "La recherche utilisateur a échoué." });
            }

            utilisateur =
                await reponse.Content.ReadFromJsonAsync<User>();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Impossible de contacter l'API de gestion.");

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new { Message = "Le service utilisateur est indisponible." });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(
                StatusCodes.Status504GatewayTimeout,
                new { Message = "Le service utilisateur ne répond pas à temps." });
        }

        if (utilisateur == null)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                new { Message = "La réponse utilisateur est vide." });
        }

        var cle = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException(
                "La clé JWT est manquante.");

        var cleSignature = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(cle));

        var signature = new SigningCredentials(
            cleSignature,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                utilisateur.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                utilisateur.Login),

            new Claim(
                ClaimTypes.Role,
                utilisateur.Role.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var expiration = DateTime.UtcNow.AddMinutes(15);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: signature);

        var tokenTexte =
            new JwtSecurityTokenHandler().WriteToken(token);

        _logger.LogInformation(
            "Token généré pour l'utilisateur {Login}.",
            utilisateur.Login);

        return Ok(new
        {
            Token = tokenTexte,
            Expiration = expiration
        });
    }
}