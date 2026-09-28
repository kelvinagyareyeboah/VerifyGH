using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Models;

// ── Adjei David Boafo (22046873) — Database Design ──────────────────────────

/// <summary>Student capstone project submission.</summary>
public class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? RepositoryUrl  { get; set; }
    public string? LiveDemoUrl    { get; set; }
    public string? DocumentUrl    { get; set; }

    /// <summary>Pipe-separated skill tags, e.g. "C#|Blazor|SQL"</summary>
    public string SkillsTags { get; set; } = string.Empty;

    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;

    public string? LecturerFeedback { get; set; }
    public DateTime? VerifiedAt     { get; set; }
    public DateTime  CreatedAt      { get; set; } = DateTime.UtcNow;

    // ── Foreign Keys ──────────────────────────────────────────────────────────

    public string  StudentUserId        { get; set; } = string.Empty;
    public string? SupervisorLecturerId { get; set; }

    // ── Navigation Properties ─────────────────────────────────────────────────

    public ApplicationUser  Student              { get; set; } = null!;
    public ApplicationUser? SupervisorLecturer   { get; set; }

    public ICollection<VerificationReview> Reviews { get; set; } = new List<VerificationReview>();
    public ICollection<ProjectSkill>       ProjectSkills { get; set; } = new List<ProjectSkill>();
}
