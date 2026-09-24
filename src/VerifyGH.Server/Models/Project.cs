using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RepositoryUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public string SkillsTags { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string StudentUserId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }

    public string? SupervisorLecturerId { get; set; }
    public ApplicationUser? SupervisorLecturer { get; set; }

    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? LecturerFeedback { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
