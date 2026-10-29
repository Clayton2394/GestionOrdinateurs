using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Blazored.SessionStorage;
using GestionOrdinateurs.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddHttpClient("GestionOrdinateursApi", client =>
    client.BaseAddress = new Uri("http://localhost:5010/"))
    .AddHttpMessageHandler(sp =>
        sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: new[] { "http://localhost:5010" }));
builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority =
        "http://localhost:8180/realms/gestion-ordinateurs";
    options.ProviderOptions.ClientId = "gestionordinateurs-client";
    options.ProviderOptions.ResponseType = "code";
});

await builder.Build().RunAsync();
