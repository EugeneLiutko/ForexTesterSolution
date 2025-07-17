using MongoDB.Bson;
using ProjectService.Application.DTOs;
using ProjectService.Application.Extensions;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;
using Xunit;

namespace ProjectService.Tests.Extensions;

public class MappingExtensionsTests
{
    [Fact]
    public void ToDto_Project_ReturnsCorrectProjectDto()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        var project = new Project
        {
            Id = projectId,
            UserId = 123,
            Name = "Test Project",
            Charts = new List<Chart>
            {
                new Chart
                {
                    Symbol = Symbol.EURUSD,
                    Timeframe = Timeframe.H1,
                    Indicators = new List<Indicator>
                    {
                        new Indicator { Name = IndicatorName.MA, Parameters = "period=20" }
                    }
                }
            }
        };

        // Act
        var result = project.ToDto();

        // Assert
        Assert.Equal(projectId.ToString(), result.Id);
        Assert.Equal(123, result.UserId);
        Assert.Equal("Test Project", result.Name);
        Assert.Single(result.Charts);
        Assert.Equal("EURUSD", result.Charts[0].Symbol);
        Assert.Equal("H1", result.Charts[0].Timeframe);
        Assert.Single(result.Charts[0].Indicators);
        Assert.Equal("MA", result.Charts[0].Indicators[0].Name);
        Assert.Equal("period=20", result.Charts[0].Indicators[0].Parameters);
    }

    [Fact]
    public void ToEntity_CreateProjectRequest_ReturnsCorrectProject()
    {
        // Arrange
        var request = new CreateProjectRequest(
            UserId: 456,
            Name: "New Project",
            Charts: new List<ChartDto>
            {
                new ChartDto(
                    Symbol: "USDJPY",
                    Timeframe: "M5",
                    Indicators: new List<IndicatorDto>
                    {
                        new IndicatorDto("RSI", "period=14")
                    }
                )
            }
        );

        // Act
        var result = request.ToEntity();

        // Assert
        Assert.Equal(456, result.UserId);
        Assert.Equal("New Project", result.Name);
        Assert.Single(result.Charts);
        Assert.Equal(Symbol.USDJPY, result.Charts[0].Symbol);
        Assert.Equal(Timeframe.M5, result.Charts[0].Timeframe);
        Assert.Single(result.Charts[0].Indicators);
        Assert.Equal(IndicatorName.RSI, result.Charts[0].Indicators[0].Name);
        Assert.Equal("period=14", result.Charts[0].Indicators[0].Parameters);
    }

    [Fact]
    public void ToDto_Chart_ReturnsCorrectChartDto()
    {
        // Arrange
        var chart = new Chart
        {
            Symbol = Symbol.EURUSD,
            Timeframe = Timeframe.H1,
            Indicators = new List<Indicator>
            {
                new Indicator { Name = IndicatorName.BB, Parameters = "period=20;dev=2" }
            }
        };

        // Act
        var result = chart.ToDto();

        // Assert
        Assert.Equal("EURUSD", result.Symbol);
        Assert.Equal("H1", result.Timeframe);
        Assert.Single(result.Indicators);
        Assert.Equal("BB", result.Indicators[0].Name);
        Assert.Equal("period=20;dev=2", result.Indicators[0].Parameters);
    }

    [Fact]
    public void ToEntity_ChartDto_ReturnsCorrectChart()
    {
        // Arrange
        var chartDto = new ChartDto(
            Symbol: "USDJPY",
            Timeframe: "M1",
            Indicators: new List<IndicatorDto>
            {
                new IndicatorDto("Ichimoku", "tenkan=9;kijun=26")
            }
        );

        // Act
        var result = chartDto.ToEntity();

        // Assert
        Assert.Equal(Symbol.USDJPY, result.Symbol);
        Assert.Equal(Timeframe.M1, result.Timeframe);
        Assert.Single(result.Indicators);
        Assert.Equal(IndicatorName.Ichimoku, result.Indicators[0].Name);
        Assert.Equal("tenkan=9;kijun=26", result.Indicators[0].Parameters);
    }

    [Fact]
    public void ToDto_Indicator_ReturnsCorrectIndicatorDto()
    {
        // Arrange
        var indicator = new Indicator
        {
            Name = IndicatorName.RSI,
            Parameters = "period=14;overbought=70"
        };

        // Act
        var result = indicator.ToDto();

        // Assert
        Assert.Equal("RSI", result.Name);
        Assert.Equal("period=14;overbought=70", result.Parameters);
    }

    [Fact]
    public void ToEntity_IndicatorDto_ReturnsCorrectIndicator()
    {
        // Arrange
        var indicatorDto = new IndicatorDto("MA", "period=50;method=EMA");

        // Act
        var result = indicatorDto.ToEntity();

        // Assert
        Assert.Equal(IndicatorName.MA, result.Name);
        Assert.Equal("period=50;method=EMA", result.Parameters);
    }
}
