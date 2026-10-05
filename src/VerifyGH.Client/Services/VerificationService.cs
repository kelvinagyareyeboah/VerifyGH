using System.Net.Http.Json;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Client.Services;

/// <summary>
/// Fetches pending projects for lecturers, submits reviews, and
/// maintains the SignalR connection for real-time status notifications.
/// </summary>
public class VerificationService : IAsyncDisposable
{
    private readonly HttpClient  _http;
    private readonly ISnackbar   _snackbar;
    private readonly AuthService _auth;

    private HubConnection? _hubConnection;

    public VerificationService(HttpClient http, ISnackbar snackbar, AuthService auth)
    {
        _http     = http;
        _snackbar = snackbar;
        _auth     = auth;
    }

    // ── SignalR Hub ───────────────────────────────────────────────────────────

    /// <summary>Connect to the notifications hub and listen for project status updates.</summary>
    public async Task StartSignalRAsync()
    {
        if (_hubConnection is not null) return;

        var token = await _auth.GetTokenAsync();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{_http.BaseAddress}hubs/notifications", options =>
            {
                if (!string.IsNullOrEmpty(token))
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()
            .Build();

        // Listen for project status changes
        _hubConnection.On<int, string>("ProjectStatusUpdated", (projectId, newStatus) =>
        {
            _snackbar.Add(
                $"Project #{projectId} status updated to: {newStatus}",
                Severity.Info,
                config =>
                {
                    config.VisibleStateDuration = 5000;
                    config.ShowCloseIcon        = true;
                });
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch
        {
            // Hub connection is best-effort; silently fail if backend is offline
            _hubConnection = null;
        }
    }

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

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

    // ── GET /api/verification/pending ─────────────────────────────────────────

    public async Task<List<ProjectDto>> GetPendingSubmissionsAsync(string? lecturerEmail = null)
    {
        try
        {
            await AttachTokenAsync();
            var query = !string.IsNullOrWhiteSpace(lecturerEmail) ? $"?lecturer={Uri.EscapeDataString(lecturerEmail)}" : "";
            return await _http.GetFromJsonAsync<List<ProjectDto>>($"api/verification/pending{query}")
                   ?? new List<ProjectDto>();
        }
        catch
        {
            return new List<ProjectDto>();
        }
    }

    // ── POST /api/verification/{id}/review ────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> SubmitReviewAsync(VerifyProjectDto dto)
    {
        try
        {
            await AttachTokenAsync();
            var response = await _http.PostAsJsonAsync($"api/verification/{dto.ProjectId}/review", dto);

            if (response.IsSuccessStatusCode)
            {
                var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
                return (project, null);
            }

            var err = await response.Content.ReadFromJsonAsync<ApiError>();
            return (null, err?.Message ?? "Review submission failed.");
        }
        catch (Exception ex)
        {
            return (null, $"Network error: {ex.Message}");
        }
    }

    // ── Cleanup ───────────────────────────────────────────────────────────────

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }

    private record ApiError(string? Message);
}
