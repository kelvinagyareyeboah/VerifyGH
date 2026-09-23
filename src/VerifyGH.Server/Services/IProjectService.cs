using VerifyGH.Shared.DTOs;

namespace VerifyGH.Server.Services;

public interface IProjectService
{
    Task<List<ProjectDto>> GetProjectsAsync(string? status, string? skill);
    Task<ProjectDto?> GetProjectByIdAsync(int id);
    Task<(ProjectDto? project, string? error)> CreateProjectAsync(CreateProjectDto dto, string studentUserId);
    Task<(ProjectDto? project, string? error)> UpdateProjectAsync(int id, CreateProjectDto dto, string requestingUserId);
    Task<(bool success, string? error)> DeleteProjectAsync(int id, string requestingUserId);
    Task<List<ProjectDto>> GetPendingForLecturerAsync(string lecturerUserId);
    Task<(ProjectDto? project, string? error)> ReviewProjectAsync(int id, VerifyProjectDto dto, string lecturerUserId);
}
