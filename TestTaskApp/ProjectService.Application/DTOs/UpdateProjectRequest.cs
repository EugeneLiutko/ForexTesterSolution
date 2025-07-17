namespace ProjectService.Application.DTOs;

public record UpdateProjectRequest(
    string Name,
    List<ChartDto>? Charts = null
);