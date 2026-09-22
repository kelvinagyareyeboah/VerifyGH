using Microsoft.AspNetCore.Identity;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Server.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string? Institution { get; set; }
    public string? Department { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
