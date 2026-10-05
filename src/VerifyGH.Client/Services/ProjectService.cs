using System.Net.Http.Json;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Client.Services;

/// <summary>Fetches and submits student projects via the backend API.</summary>
public class ProjectService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;

    public ProjectService(HttpClient http, AuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    private async Task AttachTokenAsync()
    {
        try
        {
            var token = await _auth.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch
        {
            // Ignore if storage is inaccessible
        }
    }

    // ── GET /api/projects ─────────────────────────────────────────────────────

    public async Task<List<ProjectDto>> GetProjectsAsync(string? status = null, string? skill = null)
    {
        try
        {
            var query = BuildQuery(("status", status), ("skill", skill));
            return await _http.GetFromJsonAsync<List<ProjectDto>>($"api/projects{query}")
                   ?? new List<ProjectDto>();
        }
        catch
        {
            return new List<ProjectDto>();
        }
    }

    // ── GET /api/projects/{id} ────────────────────────────────────────────────

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        try
        {
            return await _http.GetFromJsonAsync<ProjectDto>($"api/projects/{id}");
        }
        catch
        {
            return null;
        }
    }

    // ── POST /api/projects ────────────────────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> CreateProjectAsync(CreateProjectDto dto)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync("api/projects", dto);
            if (response.IsSuccessStatusCode)
            {
                var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
                return (project, null);
            }

            var err = await response.Content.ReadFromJsonAsync<ApiError>();
            return (null, err?.Message ?? "Submission failed.");
        }
        catch (Exception ex)
        {
            return (null, $"Network error: {ex.Message}");
        }
    }

    // ── PUT /api/projects/{id} ────────────────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> UpdateProjectAsync(int id, CreateProjectDto dto)
    {
        try
        {
            var response = await _http.PutAsJsonAsync($"api/projects/{id}", dto);
            if (response.IsSuccessStatusCode)
            {
                var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
                return (project, null);
            }

            var err = await response.Content.ReadFromJsonAsync<ApiError>();
            return (null, err?.Message ?? "Update failed.");
        }
        catch (Exception ex)
        {
            return (null, $"Network error: {ex.Message}");
        }
    }

    // ── DELETE /api/projects/{id} ─────────────────────────────────────────────

    public async Task<(bool success, string? error)> DeleteProjectAsync(int id)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/projects/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var err = await response.Content.ReadFromJsonAsync<ApiError>();
            return (false, err?.Message ?? "Delete failed.");
        }
        catch (Exception ex)
        {
            return (false, $"Network error: {ex.Message}");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string BuildQuery(params (string key, string? value)[] pairs)
    {
        var parts = pairs
            .Where(p => !string.IsNullOrWhiteSpace(p.value))
            .Select(p => $"{p.key}={Uri.EscapeDataString(p.value!)}");
        var qs = string.Join("&", parts);
        return qs.Length > 0 ? $"?{qs}" : string.Empty;
    }

    private record ApiError(string? Message);
}
