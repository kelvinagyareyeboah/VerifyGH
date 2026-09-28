using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Models;

/// <summary>Lecturer review record for a project submission.</summary>
public class VerificationReview
{
    public int Id { get; set; }

    public VerificationStatus Status   { get; set; }
    public string?            Comments { get; set; }
    public DateTime           ReviewedAt { get; set; } = DateTime.UtcNow;

    // ── Foreign Keys ──────────────────────────────────────────────────────────

    public int    ProjectId    { get; set; }
    public string LecturerId   { get; set; } = string.Empty;

    // ── Navigation Properties ─────────────────────────────────────────────────

    public Project         Project  { get; set; } = null!;
    public ApplicationUser Lecturer { get; set; } = null!;
}

/// <summary>Skill catalogue entry (e.g. "C#", "React", "Machine Learning").</summary>
public class Skill
{
    public int    Id       { get; set; }
    public string Name     { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;  // e.g. "Backend", "Frontend", "AI/ML"

    public ICollection<ProjectSkill> ProjectSkills { get; set; } = new List<ProjectSkill>();
}

/// <summary>Many-to-many join between Project and Skill.</summary>
public class ProjectSkill
{
    public int ProjectId { get; set; }
    public int SkillId   { get; set; }

    public Project Project { get; set; } = null!;
    public Skill   Skill   { get; set; } = null!;
}
