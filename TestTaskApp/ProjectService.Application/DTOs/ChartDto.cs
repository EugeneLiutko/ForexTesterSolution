namespace ProjectService.Application.DTOs;

public record ChartDto(
    string Symbol,
    string Timeframe,
    List<IndicatorDto> Indicators
);