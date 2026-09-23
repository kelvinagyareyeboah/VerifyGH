using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VerifyGH.Server.Services;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    // ── GET /api/projects?status=Pending&skill=React ──────────────────────────

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects(
        [FromQuery] string? status,
        [FromQuery] string? skill)
    {
        var projects = await _projectService.GetProjectsAsync(status, skill);
        return Ok(projects);
    }

    // ── GET /api/projects/{id} ────────────────────────────────────────────────

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);

        if (project is null)
            return NotFound(new { message = $"Project with ID {id} was not found." });

        return Ok(project);
    }

    // ── POST /api/projects ────────────────────────────────────────────────────

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { message = "Validation failed.", errors });
        }

        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var (project, error) = await _projectService.CreateProjectAsync(dto, studentUserId);

        if (error is not null)
            return BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetProjectById), new { id = project!.Id }, project);
    }

    // ── PUT /api/projects/{id} ────────────────────────────────────────────────

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] CreateProjectDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return BadRequest(new { message = "Validation failed.", errors });
        }

        var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(requestingUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var (project, error) = await _projectService.UpdateProjectAsync(id, dto, requestingUserId);

        if (error is null) return Ok(project);

        return error switch
        {
            var e when e.Contains("not found")        => NotFound(new { message = e }),
            var e when e.Contains("not authorised")   => StatusCode(StatusCodes.Status403Forbidden, new { message = e }),
            _                                          => BadRequest(new { message = error })
        };
    }

    // ── DELETE /api/projects/{id} ─────────────────────────────────────────────

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(requestingUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var (success, error) = await _projectService.DeleteProjectAsync(id, requestingUserId);

        if (success) return NoContent();

        return error switch
        {
            var e when e.Contains("not found")        => NotFound(new { message = e }),
            var e when e.Contains("not authorised")   => StatusCode(StatusCodes.Status403Forbidden, new { message = e }),
            _                                          => BadRequest(new { message = error })
        };
    }
}
