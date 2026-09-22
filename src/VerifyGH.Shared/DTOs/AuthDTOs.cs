using System.ComponentModel.DataAnnotations;
using VerifyGH.Shared.Enums;

namespace VerifyGH.Shared.DTOs;

public class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    public string? StudentId { get; set; }

    public string? Institution { get; set; }

    public string? Department { get; set; }

    public UserRole Role { get; set; } = UserRole.Student;
}

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? Message { get; set; }
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string? Institution { get; set; }
    public string? Department { get; set; }
    public UserRole Role { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int VerifiedProjectsCount { get; set; }
}
