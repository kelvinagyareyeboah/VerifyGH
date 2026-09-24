using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;
using VerifyGH.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// MudBlazor Component Services
builder.Services.AddMudServices();

// Local Storage for Auth Tokens
builder.Services.AddBlazoredLocalStorage();

// Authorization Core for Client-side Role Checks
builder.Services.AddAuthorizationCore();

// In-Memory Role and Session State
builder.Services.AddScoped<VerifyGH.Client.Services.UserSessionService>();

// Configure HttpClient pointing to Backend Web API
var backendApiUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:7296/";
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendApiUrl)
});

await builder.Build().RunAsync();
