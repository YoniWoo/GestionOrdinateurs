using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GestionOrdinateurs.Client.Services;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ISessionStorageService _sessionStorage;

    public JwtAuthenticationStateProvider(
        ISessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        var token = await _sessionStorage
            .GetItemAsync<string>("jwtToken");

        var etat = ConstruireEtat(token);

        if (!etat.User.Identity!.IsAuthenticated
            && !string.IsNullOrWhiteSpace(token))
        {
            await _sessionStorage.RemoveItemAsync("jwtToken");
        }

        return etat;
    }

    public void NotifierConnexion(string token)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(ConstruireEtat(token)));
    }

    public void NotifierDeconnexion()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(ConstruireEtat(null)));
    }

    private static AuthenticationState ConstruireEtat(string? token)
    {
        var anonyme = new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity()));

        var lecteur = new JwtSecurityTokenHandler();

        if (string.IsNullOrWhiteSpace(token)
            || !lecteur.CanReadToken(token))
        {
            return anonyme;
        }

        var jwt = lecteur.ReadJwtToken(token);

        if (jwt.ValidTo <= DateTime.UtcNow)
        {
            return anonyme;
        }
        var claims = jwt.Claims.Select(claim =>
        {
            var type = claim.Type switch
            {
                "unique_name" => ClaimTypes.Name,
                "role" => ClaimTypes.Role,
                _ => claim.Type
            };

            return new Claim(type, claim.Value);
        });

        var identite = new ClaimsIdentity(claims, "jwt");

        return new AuthenticationState(
            new ClaimsPrincipal(identite));
    }
}