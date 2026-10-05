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

// API Integration Services (Tenkorang Julius — 22017966)
builder.Services.AddScoped<VerifyGH.Client.Services.AuthService>();
builder.Services.AddScoped<VerifyGH.Client.Services.ProjectService>();
builder.Services.AddScoped<VerifyGH.Client.Services.VerificationService>();

// Configure HttpClient pointing to Backend Web API
var configBackend = builder.Configuration["BackendUrl"];
var isLocalhost = builder.HostEnvironment.BaseAddress.Contains("localhost") || builder.HostEnvironment.BaseAddress.Contains("127.0.0.1");

var backendApiUrl = isLocalhost 
    ? "http://localhost:5019/" 
    : (!string.IsNullOrWhiteSpace(configBackend) && !configBackend.Contains("localhost") 
        ? configBackend 
        : "https://verifygh-api-lvws.onrender.com/");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(backendApiUrl),
    Timeout = TimeSpan.FromSeconds(30)
});

await builder.Build().RunAsync();
