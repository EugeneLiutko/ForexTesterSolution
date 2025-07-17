using ProjectService.Domain.Enums;

namespace ProjectService.Domain.Entities;

public class Chart
{
    public Symbol Symbol { get; set; }
    public Timeframe Timeframe { get; set; }
    public List<Indicator> Indicators { get; set; } = new();
}