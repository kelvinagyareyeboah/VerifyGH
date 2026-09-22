using System.ComponentModel.DataAnnotations;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Shared.DTOs;

public class CreateProjectDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? RepositoryUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public string? DocumentUrl { get; set; }

    public List<string> Skills { get; set; } = new();

    public string? SupervisorLecturerId { get; set; }
}

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RepositoryUrl { get; set; }
    public string? LiveDemoUrl { get; set; }
    public string? DocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? Institution { get; set; }

    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public string? SupervisorLecturerId { get; set; }
    public string? SupervisorName { get; set; }
    public string? LecturerFeedback { get; set; }
    public DateTime? VerifiedAt { get; set; }

    public List<string> Skills { get; set; } = new();
}

public class VerifyProjectDto
{
    public int ProjectId { get; set; }
    public VerificationStatus Status { get; set; }
    public string? Feedback { get; set; }
}
