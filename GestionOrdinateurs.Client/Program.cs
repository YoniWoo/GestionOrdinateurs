using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestionOrdinateurs.Client;
using GestionOrdinateurs.Client.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(
    services => services
        .GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddHttpClient("Authentification", client =>
{
    client.BaseAddress = new Uri("http://localhost:5006/");
});

builder.Services.AddTransient<JwtHandler>();
builder.Services.AddHttpClient("GestionOrdinateurs", client =>
{
    client.BaseAddress = new Uri("http://localhost:5010/");
})
.AddHttpMessageHandler<JwtHandler>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
