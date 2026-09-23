using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VerifyGH.Server.Data;
using VerifyGH.Server.Models;
using VerifyGH.Shared.DTOs;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Services;

public class ProjectService : IProjectService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static List<string> SplitSkills(string raw)
        => string.IsNullOrWhiteSpace(raw)
            ? new List<string>()
            : raw.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

    private static string JoinSkills(List<string> skills)
        => string.Join('|', skills.Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)));

    private static ProjectDto MapToDto(Project p) => new()
    {
        Id               = p.Id,
        Title            = p.Title,
        Description      = p.Description,
        RepositoryUrl    = p.RepositoryUrl,
        LiveDemoUrl      = p.LiveDemoUrl,
        DocumentUrl      = p.DocumentUrl,
        CreatedAt        = p.CreatedAt,
        StudentId        = p.StudentUserId,
        StudentName      = p.Student?.FullName ?? string.Empty,
        Institution      = p.Student?.Institution,
        Status           = p.Status,
        SupervisorLecturerId = p.SupervisorLecturerId,
        SupervisorName   = p.SupervisorLecturer?.FullName,
        LecturerFeedback = p.LecturerFeedback,
        VerifiedAt       = p.VerifiedAt,
        Skills           = SplitSkills(p.SkillsTags)
    };

    // ── GET /api/projects ─────────────────────────────────────────────────────

    public async Task<List<ProjectDto>> GetProjectsAsync(string? status, string? skill)
    {
        var query = _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<VerificationStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(p => p.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(skill))
        {
            query = query.Where(p => p.SkillsTags.Contains(skill));
        }

        var projects = await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        return projects.Select(MapToDto).ToList();
    }

    // ── GET /api/projects/{id} ────────────────────────────────────────────────

    public async Task<ProjectDto?> GetProjectByIdAsync(int id)
    {
        var project = await _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .FirstOrDefaultAsync(p => p.Id == id);

        return project is null ? null : MapToDto(project);
    }

    // ── POST /api/projects ────────────────────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> CreateProjectAsync(
        CreateProjectDto dto, string studentUserId)
    {
        // Verify the supervisor exists and is a Lecturer if provided
        if (!string.IsNullOrWhiteSpace(dto.SupervisorLecturerId))
        {
            var supervisor = await _userManager.FindByIdAsync(dto.SupervisorLecturerId);
            if (supervisor is null || supervisor.Role != UserRole.Lecturer)
                return (null, "The specified supervisor lecturer was not found.");
        }

        var project = new Project
        {
            Title               = dto.Title,
            Description         = dto.Description,
            RepositoryUrl       = dto.RepositoryUrl,
            LiveDemoUrl         = dto.LiveDemoUrl,
            DocumentUrl         = dto.DocumentUrl,
            SkillsTags          = JoinSkills(dto.Skills),
            SupervisorLecturerId = dto.SupervisorLecturerId,
            StudentUserId       = studentUserId,
            Status              = VerificationStatus.Pending,
            CreatedAt           = DateTime.UtcNow
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Reload with navigation properties for the response
        var created = await _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .FirstAsync(p => p.Id == project.Id);

        return (MapToDto(created), null);
    }

    // ── PUT /api/projects/{id} ────────────────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> UpdateProjectAsync(
        int id, CreateProjectDto dto, string requestingUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return (null, "Project not found.");

        if (project.StudentUserId != requestingUserId)
            return (null, "You are not authorised to edit this submission.");

        if (project.Status != VerificationStatus.Pending)
            return (null, "Cannot edit a submission that is under review or has been decided.");

        if (!string.IsNullOrWhiteSpace(dto.SupervisorLecturerId))
        {
            var supervisor = await _userManager.FindByIdAsync(dto.SupervisorLecturerId);
            if (supervisor is null || supervisor.Role != UserRole.Lecturer)
                return (null, "The specified supervisor lecturer was not found.");
        }

        project.Title                = dto.Title;
        project.Description          = dto.Description;
        project.RepositoryUrl        = dto.RepositoryUrl;
        project.LiveDemoUrl          = dto.LiveDemoUrl;
        project.DocumentUrl          = dto.DocumentUrl;
        project.SkillsTags           = JoinSkills(dto.Skills);
        project.SupervisorLecturerId = dto.SupervisorLecturerId;

        await _context.SaveChangesAsync();
        return (MapToDto(project), null);
    }

    // ── DELETE /api/projects/{id} ─────────────────────────────────────────────

    public async Task<(bool success, string? error)> DeleteProjectAsync(
        int id, string requestingUserId)
    {
        var project = await _context.Projects.FindAsync(id);

        if (project is null)
            return (false, "Project not found.");

        if (project.StudentUserId != requestingUserId)
            return (false, "You are not authorised to delete this submission.");

        if (project.Status != VerificationStatus.Pending)
            return (false, "Cannot delete a submission that is under review or has been decided.");

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    // ── GET /api/verification/pending ─────────────────────────────────────────

    public async Task<List<ProjectDto>> GetPendingForLecturerAsync(string lecturerUserId)
    {
        var projects = await _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .Where(p => p.SupervisorLecturerId == lecturerUserId &&
                        p.Status == VerificationStatus.Pending)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();

        return projects.Select(MapToDto).ToList();
    }

    // ── POST /api/verification/{id}/review ────────────────────────────────────

    public async Task<(ProjectDto? project, string? error)> ReviewProjectAsync(
        int id, VerifyProjectDto dto, string lecturerUserId)
    {
        var project = await _context.Projects
            .Include(p => p.Student)
            .Include(p => p.SupervisorLecturer)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null)
            return (null, "Project not found.");

        if (project.SupervisorLecturerId != lecturerUserId)
            return (null, "This submission is not assigned to you.");

        var allowedStatuses = new[]
        {
            VerificationStatus.Approved,
            VerificationStatus.Rejected,
            VerificationStatus.NeedsRevision
        };

        if (!allowedStatuses.Contains(dto.Status))
            return (null, "Invalid review status. Allowed values: Approved, Rejected, NeedsRevision.");

        project.Status           = dto.Status;
        project.LecturerFeedback = dto.Feedback;
        project.VerifiedAt       = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return (MapToDto(project), null);
    }
}
