namespace ProjectService.Application.DTOs;

public record CreateProjectRequest(
    int UserId,
    string Name,
    List<ChartDto>? Charts = null
);