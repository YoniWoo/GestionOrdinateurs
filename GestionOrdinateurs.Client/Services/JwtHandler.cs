using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace GestionOrdinateurs.Client.Services;

public class JwtHandler : AuthorizationMessageHandler
{
    public JwtHandler(
        IAccessTokenProvider fournisseurToken,
        NavigationManager navigation)
        : base(fournisseurToken, navigation)
    {
        ConfigureHandler(
            authorizedUrls: new[]
            {
                "http://localhost:5010/"
            });
    }
}