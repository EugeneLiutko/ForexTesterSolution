using MongoDB.Bson;
using ProjectService.Application.DTOs;
using ProjectService.Application.Repositories;
using ProjectService.Domain.Entities;
using Microsoft.Extensions.Logging;
using ProjectService.Application.Extensions;

namespace ProjectService.Application.Services;

public class ProjectService : IProjectService
{
   private readonly IProjectRepository _projectRepository;
   private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProjectDto> GetProjectByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid project ID format", nameof(id));
        }

        var project = await _projectRepository.GetByIdAsync(objectId);
        if (project == null)
        {
            throw new KeyNotFoundException($"Project with ID {id} not found");
        }

        return project.ToDto();
    }

    public async Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be positive", nameof(userId));
        }

        var projects = await _projectRepository.GetByUserIdAsync(userId);
        return projects.Select(p => p.ToDto());
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(p => p.ToDto());
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Project name is required", nameof(request));
        }

        if (request.UserId <= 0)
        {
            throw new ArgumentException("User ID must be positive", nameof(request));
        }

        var project = request.ToEntity();
        await _projectRepository.AddAsync(project);
        
        _logger.LogInformation("Created project with ID: {ProjectId} for user: {UserId}", project.Id, project.UserId);
        
        return project.ToDto();
    }

    public async Task<ProjectDto> UpdateProjectAsync(string id, UpdateProjectRequest request)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid project ID format", nameof(id));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Project name is required", nameof(request));
        }

        var existingProject = await _projectRepository.GetByIdAsync(objectId);
        if (existingProject == null)
        {
            throw new KeyNotFoundException($"Project with ID {id} not found");
        }

        existingProject.Name = request.Name;
        existingProject.Charts = request.Charts?.Select(c => c.ToEntity()).ToList() ?? new List<Chart>();

        await _projectRepository.UpdateAsync(existingProject);
        
        _logger.LogInformation("Updated project with ID: {ProjectId}", id);
        
        return existingProject.ToDto();
    }

    public async Task DeleteProjectAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid project ID format", nameof(id));
        }

        var existingProject = await _projectRepository.GetByIdAsync(objectId);
        if (existingProject == null)
        {
            throw new KeyNotFoundException($"Project with ID {id} not found");
        }

        await _projectRepository.DeleteAsync(objectId);
        
        _logger.LogInformation("Deleted project with ID: {ProjectId}", id);
    }
}