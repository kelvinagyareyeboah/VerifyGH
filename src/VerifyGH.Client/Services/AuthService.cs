using Blazored.LocalStorage;
using System.Net.Http.Json;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Client.Services;

// ── Tenkorang Julius (22017966) — API Integration ────────────────────────────

/// <summary>Handles login, registration, JWT storage and current user profile.</summary>
public class AuthService
{
    private readonly HttpClient          _http;
    private readonly ILocalStorageService _storage;
    private readonly UserSessionService  _session;

    private const string TokenKey = "auth_token";

    public AuthService(HttpClient http, ILocalStorageService storage, UserSessionService session)
    {
        _http    = http;
        _storage = storage;
        _session = session;
    }

    // ── Register ──────────────────────────────────────────────────────────────

    public async Task<(bool success, string? error)> RegisterAsync(RegisterRequestDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", dto);
            if (response.IsSuccessStatusCode)
                return (true, null);

            var problem = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return (false, problem?.Message ?? "Registration failed.");
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public async Task<(bool success, string? error)> LoginAsync(LoginRequestDto dto)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, problem?.Message ?? "Login failed. Check your credentials.");
            }

            var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (auth is null || string.IsNullOrEmpty(auth.Token))
                return (false, "Invalid response from server.");

            // Persist JWT
            await _storage.SetItemAsync(TokenKey, auth.Token);

            // Update the in-memory session
            _session.Login(auth.Email, auth.Role, auth.FullName);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    public async Task LogoutAsync()
    {
        await _storage.RemoveItemAsync(TokenKey);
        _session.Logout();
    }

    // ── Get stored token ──────────────────────────────────────────────────────

    public async Task<string?> GetTokenAsync()
        => await _storage.GetItemAsync<string>(TokenKey);

    // ── Get user profile ──────────────────────────────────────────────────────

    public async Task<UserProfileDto?> GetProfileAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<UserProfileDto>("api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private record ErrorResponse(string? Message);
}
