using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

public class Indicator
{
    public IndicatorName Name { get; set; }
    public string Parameters { get; set; }
}