using GestionOrdinateurs.Client;
using GestionOrdinateurs.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority =
        "http://localhost:8180/realms/gestion-ordinateurs";

    options.ProviderOptions.ClientId =
        "gestion-ordinateurs-client";

    options.ProviderOptions.ResponseType = "code";

    options.UserOptions.NameClaim = "preferred_username";
    options.UserOptions.RoleClaim = "role";

    options.ProviderOptions.PostLogoutRedirectUri =
    "http://localhost:5000/authentication/logout-callback";
});

builder.Services.AddTransient<JwtHandler>();

builder.Services.AddHttpClient("GestionOrdinateurs", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/");
})
.AddHttpMessageHandler<JwtHandler>();

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();