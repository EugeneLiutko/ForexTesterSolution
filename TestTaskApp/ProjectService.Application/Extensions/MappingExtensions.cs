using ProjectService.Application.DTOs;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;

namespace ProjectService.Application.Extensions;

public static class MappingExtensions
{
    // Project mappings
    public static ProjectDto ToDto(this Project project)
    {
        return new ProjectDto(
            project.Id.ToString(),
            project.UserId,
            project.Name,
            project.Charts.Select(c => c.ToDto()).ToList()
        );
    }

    public static Project ToEntity(this CreateProjectRequest request)
    {
        return new Project
        {
            UserId = request.UserId,
            Name = request.Name,
            Charts = request.Charts?.Select(c => c.ToEntity()).ToList() ?? new List<Chart>()
        };
    }

    // Chart mappings
    public static ChartDto ToDto(this Chart chart)
    {
        return new ChartDto(
            chart.Symbol.ToString(),
            chart.Timeframe.ToString(),
            chart.Indicators.Select(i => i.ToDto()).ToList()
        );
    }

    public static Chart ToEntity(this ChartDto chartDto)
    {
        return new Chart
        {
            Symbol = Enum.Parse<Symbol>(chartDto.Symbol),
            Timeframe = Enum.Parse<Timeframe>(chartDto.Timeframe),
            Indicators = chartDto.Indicators.Select(i => i.ToEntity()).ToList()
        };
    }

    // Indicator mappings
    public static IndicatorDto ToDto(this Indicator indicator)
    {
        return new IndicatorDto(
            indicator.Name.ToString(),
            indicator.Parameters
        );
    }

    public static Indicator ToEntity(this IndicatorDto indicatorDto)
    {
        return new Indicator
        {
            Name = Enum.Parse<IndicatorName>(indicatorDto.Name),
            Parameters = indicatorDto.Parameters
        };
    }

    // UserSettings mappings
    public static UserSettingsDto ToDto(this UserSettings userSettings)
    {
        return new UserSettingsDto(
            userSettings.Id.ToString(),
            userSettings.UserId,
            userSettings.Language.ToString(),
            userSettings.Theme.ToString()
        );
    }

    public static UserSettings ToEntity(this CreateUserSettingsRequest request)
    {
        return new UserSettings
        {
            UserId = request.UserId,
            Language = Enum.Parse<Language>(request.Language),
            Theme = Enum.Parse<Theme>(request.Theme)
        };
    }
}