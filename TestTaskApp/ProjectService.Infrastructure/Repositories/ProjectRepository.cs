using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectService.Application.Repositories;
using ProjectService.Domain.Entities;
using ProjectService.Infrastructure.Persistence;

namespace ProjectService.Infrastructure.Repositories;

public class ProjectRepository: IProjectRepository
{
   private readonly IMongoCollection<Project> _projects;
    private readonly ILogger<ProjectRepository> _logger;

    public ProjectRepository(MongoDbContext context, ILogger<ProjectRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        if (context == null)
        {
            _logger.LogError("MongoDbContext is null");
            throw new ArgumentNullException(nameof(context));
        }

        _projects = context.Projects;
        
        if (_projects == null)
        {
            _logger.LogError("Projects collection is null");
            throw new InvalidOperationException("Projects collection is not initialized");
        }
        
        _logger.LogDebug("ProjectRepository initialized successfully");
    }

    public async Task<Project> GetByIdAsync(ObjectId id)
    {
        try
        {
            _logger.LogDebug("Getting project by ID: {ProjectId}", id);
            
            var project = await _projects.Find(p => p.Id == id).FirstOrDefaultAsync();
            
            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
            }
            else
            {
                _logger.LogDebug("Found project: {ProjectName}", project.Name);
            }
            
            return project;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project by ID: {ProjectId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Project>> GetByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogDebug("Getting projects for user ID: {UserId}", userId);
            
            var projects = await _projects.Find(p => p.UserId == userId).ToListAsync();
            
            _logger.LogDebug("Found {ProjectCount} projects for user ID: {UserId}", projects.Count, userId);
            
            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Getting all projects");
            
            var projects = await _projects.Find(_ => true).ToListAsync();
            
            _logger.LogDebug("Found {ProjectCount} total projects", projects.Count);
            
            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all projects");
            throw;
        }
    }

    public async Task AddAsync(Project project)
    {
        try
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            _logger.LogDebug("Adding project: {ProjectName} for user: {UserId}", project.Name, project.UserId);
            
            await _projects.InsertOneAsync(project);
            
            _logger.LogInformation("Successfully added project with ID: {ProjectId}", project.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding project: {ProjectName} for user: {UserId}", 
                project?.Name, project?.UserId);
            throw;
        }
    }

    public async Task UpdateAsync(Project project)
    {
        try
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            _logger.LogDebug("Updating project with ID: {ProjectId}", project.Id);
            
            var result = await _projects.ReplaceOneAsync(p => p.Id == project.Id, project);
            
            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found for update", project.Id);
                throw new KeyNotFoundException($"Project with ID {project.Id} not found");
            }
            
            _logger.LogInformation("Successfully updated project with ID: {ProjectId}", project.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project with ID: {ProjectId}", project?.Id);
            throw;
        }
    }

    public async Task DeleteAsync(ObjectId id)
    {
        try
        {
            _logger.LogDebug("Deleting project with ID: {ProjectId}", id);
            
            var result = await _projects.DeleteOneAsync(p => p.Id == id);
            
            if (result.DeletedCount == 0)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found for deletion", id);
                throw new KeyNotFoundException($"Project with ID {id} not found");
            }
            
            _logger.LogInformation("Successfully deleted project with ID: {ProjectId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Project>> GetProjectsByUserIdsAsync(IEnumerable<int> userIds)
    {
        try
        {
            var userIdList = userIds.ToList();
            _logger.LogDebug("Getting projects for {UserCount} users", userIdList.Count);
            
            var projects = await _projects.Find(p => userIdList.Contains(p.UserId)).ToListAsync();
            
            _logger.LogDebug("Found {ProjectCount} projects for specified users", projects.Count);
            
            return projects;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects for user IDs");
            throw;
        }
    }
}