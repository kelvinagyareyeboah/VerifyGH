using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VerifyGH.Server.Hubs;
using VerifyGH.Server.Models;
using VerifyGH.Server.Services;
using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectService projectService,
        IHubContext<NotificationHub, INotificationClient> hubContext,
        ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _hubContext     = hubContext;
        _logger         = logger;
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

    // ── GET /api/projects/lecturers ───────────────────────────────────────────

    [HttpGet("lecturers")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<SupervisorOptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLecturers(
        [FromServices] Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        var lecturers = await userManager.GetUsersInRoleAsync("Lecturer");
        var list = lecturers.Select(l => new SupervisorOptionDto
        {
            Id         = l.Id,
            FullName   = l.FullName,
            Email      = l.Email ?? string.Empty,
            Department = l.Department ?? "Department of Computer Science"
        }).OrderBy(l => l.FullName).ToList();

        if (list.Count == 0)
        {
            list = new List<SupervisorOptionDto>
            {
                new() { Id = "soli", FullName = "Dr. Michael Soli", Email = "msoli@ug.edu.gh", Department = "Department of Computer Science" },
                new() { Id = "wiafe", FullName = "Dr. Isaac Wiafe", Email = "iwiafe@ug.edu.gh", Department = "Department of Computer Science" }
            };
        }

        return Ok(list);
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

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject(
        [FromBody] CreateProjectDto dto,
        [FromServices] Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
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
        {
            var kelvin = await userManager.FindByEmailAsync("onlykelvin06@gmail.com");
            studentUserId = kelvin?.Id;
        }

        if (string.IsNullOrEmpty(studentUserId))
            return BadRequest(new { message = "Could not resolve a student account for this submission." });

        var (project, error) = await _projectService.CreateProjectAsync(dto, studentUserId);

        if (error is not null)
            return BadRequest(new { message = error });

        if (project is not null)
        {
            try
            {
                await _hubContext.Clients.All.NewProjectSubmitted(project);
                await _hubContext.Clients.All.ReceiveNotification(
                    "New Deliverable Submitted",
                    $"'{project.Title}' was submitted by {project.StudentName}.",
                    "info");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to broadcast NewProjectSubmitted via SignalR.");
            }
        }

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
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(
        int id,
        [FromServices] Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
    {
        var requestingUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(requestingUserId))
        {
            var kelvin = await userManager.FindByEmailAsync("onlykelvin06@gmail.com");
            requestingUserId = kelvin?.Id;
        }

        if (string.IsNullOrEmpty(requestingUserId))
            return Unauthorized(new { message = "User identity could not be determined." });

        var (success, error) = await _projectService.DeleteProjectAsync(id, requestingUserId);

        if (success)
        {
            try
            {
                await _hubContext.Clients.All.ProjectDeleted(id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to broadcast ProjectDeleted via SignalR.");
            }
            return NoContent();
        }

        return error switch
        {
            var e when e.Contains("not found")        => NotFound(new { message = e }),
            var e when e.Contains("not authorised")   => StatusCode(StatusCodes.Status403Forbidden, new { message = e }),
            _                                          => BadRequest(new { message = error })
        };
    }
}
