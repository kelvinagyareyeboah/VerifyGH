using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VerifyGH.Server.Hubs;
using VerifyGH.Server.Models;
using VerifyGH.Server.Services;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VerificationController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ILogger<VerificationController> _logger;

    public VerificationController(
        IProjectService projectService,
        UserManager<ApplicationUser> userManager,
        IHubContext<NotificationHub, INotificationClient> hubContext,
        ILogger<VerificationController> logger)
    {
        _projectService = projectService;
        _userManager    = userManager;
        _hubContext     = hubContext;
        _logger         = logger;
    }

    // ── GET /api/verification/pending ─────────────────────────────────────────

    [HttpGet("pending")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingSubmissions([FromQuery] string? lecturer = null)
    {
        var lecturerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // If not found in token, try resolving from query (email or id)
        if (string.IsNullOrEmpty(lecturerUserId) && !string.IsNullOrWhiteSpace(lecturer))
        {
            var raw = lecturer.Trim();
            var u = await _userManager.FindByIdAsync(raw)
                 ?? await _userManager.FindByEmailAsync(raw);

            if (u is null && raw.Contains("wiafe", StringComparison.OrdinalIgnoreCase))
                u = await _userManager.FindByEmailAsync("iwiafe@ug.edu.gh") ?? await _userManager.FindByEmailAsync("wiafe@ug.edu.gh");
            if (u is null && raw.Contains("soli", StringComparison.OrdinalIgnoreCase))
                u = await _userManager.FindByEmailAsync("msoli@ug.edu.gh");
            if (u is null && raw.Contains("owusu", StringComparison.OrdinalIgnoreCase))
                u = await _userManager.FindByEmailAsync("eowusu@ug.edu.gh");

            lecturerUserId = u?.Id;
        }

        // Return queue for this lecturer (or all pending if lecturer not specified)
        var pending = await _projectService.GetPendingForLecturerAsync(lecturerUserId ?? string.Empty);
        return Ok(pending);
    }

    // ── POST /api/verification/{id}/review ────────────────────────────────────

    [HttpPost("{id:int}/review")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewSubmission(
        int id,
        [FromBody] VerifyProjectDto dto,
        [FromQuery] string? reviewer = null)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { message = "Validation failed.", errors });
        }

        var lecturerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(lecturerUserId) && !string.IsNullOrWhiteSpace(reviewer))
        {
            var u = await _userManager.FindByIdAsync(reviewer)
                 ?? await _userManager.FindByEmailAsync(reviewer);
            lecturerUserId = u?.Id;
        }

        if (string.IsNullOrEmpty(lecturerUserId))
        {
            var defaultLecturer = await _userManager.FindByEmailAsync("iwiafe@ug.edu.gh")
                               ?? await _userManager.FindByEmailAsync("msoli@ug.edu.gh");
            lecturerUserId = defaultLecturer?.Id ?? string.Empty;
        }

        var (project, error) = await _projectService.ReviewProjectAsync(id, dto, lecturerUserId);

        if (error is null && project is not null)
        {
            try
            {
                await _hubContext.Clients.All.ProjectStatusUpdated(
                    project.Id,
                    project.Status.ToString(),
                    project.LecturerFeedback);

                await _hubContext.Clients.All.ReceiveNotification(
                    "Project Evaluation Updated",
                    $"'{project.Title}' status is now {project.Status}.",
                    "info");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to broadcast ProjectStatusUpdated via SignalR.");
            }
            return Ok(project);
        }

        return error switch
        {
            var e when e.Contains("not found") => NotFound(new { message = e }),
            _                                  => BadRequest(new { message = error })
        };
    }
}
