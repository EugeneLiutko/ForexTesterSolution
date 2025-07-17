using ProjectService.Application.DTOs;

namespace ProjectService.Application.Services;

public interface IProjectService
{
    Task<ProjectDto> GetProjectByIdAsync(string id);
    Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(int userId);
    Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request);
    Task<ProjectDto> UpdateProjectAsync(string id, UpdateProjectRequest request);
    Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
    Task DeleteProjectAsync(string id);
}