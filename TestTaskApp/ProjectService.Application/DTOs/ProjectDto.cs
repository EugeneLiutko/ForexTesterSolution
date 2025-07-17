namespace ProjectService.Application.DTOs;

public record ProjectDto(
    string Id,
    int UserId,
    string Name,
    List<ChartDto> Charts
);