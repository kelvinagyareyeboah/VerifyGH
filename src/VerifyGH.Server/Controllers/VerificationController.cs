using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerifyGH.Server.Services;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Lecturer")]
public class VerificationController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<VerificationController> _logger;

    public VerificationController(IProjectService projectService, ILogger<VerificationController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    // ── GET /api/verification/pending ─────────────────────────────────────────

    [HttpGet("pending")]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPendingSubmissions()
    {
        var lecturerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(lecturerUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var pending = await _projectService.GetPendingForLecturerAsync(lecturerUserId);
        return Ok(pending);
    }

    // ── POST /api/verification/{id}/review ────────────────────────────────────

    [HttpPost("{id:int}/review")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReviewSubmission(int id, [FromBody] VerifyProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { message = "Validation failed.", errors });
        }

        var lecturerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(lecturerUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var (project, error) = await _projectService.ReviewProjectAsync(id, dto, lecturerUserId);

        if (error is null) return Ok(project);

        return error switch
        {
            var e when e.Contains("not found")       => NotFound(new { message = e }),
            var e when e.Contains("not assigned")    => StatusCode(StatusCodes.Status403Forbidden, new { message = e }),
            _                                         => BadRequest(new { message = error })
        };
    }
}
