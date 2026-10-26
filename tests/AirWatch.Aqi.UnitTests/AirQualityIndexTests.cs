using AirWatch.Aqi.Contracts;
using AirWatch.Aqi.Core;
using FluentAssertions;

namespace AirWatch.Aqi.UnitTests;

public class AirQualityIndexTests
{
    private readonly IAirQualityIndex _sut = ServiceResolver.Resolve<IAirQualityIndex>();

    [Theory]
    [InlineData(Pollutant.Pm25, 9.04, 38)]
    [InlineData(Pollutant.Pm25, 12.34, 52)]
    [InlineData(Pollutant.Pm25, 12.0, 50)]
    [InlineData(Pollutant.Pm10, 54.0, 50)]
    [InlineData(Pollutant.Pm10, 54.9, 50)]
    [InlineData(Pollutant.O3, 54.9, 50)]
    public void SubIndex_ReturnsExpectedIndexForConcentration(
        Pollutant pollutant,
        double concentration,
        int expected)
    {
        _sut.SubIndex(pollutant, concentration).Should().Be(expected);
    }

    [Theory]
    [InlineData(Pollutant.Pm25, -0.1)]
    [InlineData(Pollutant.Pm10, -0.1)]
    [InlineData(Pollutant.O3, -0.1)]
    public void SubIndex_ReturnsNegativeOneForNegativeConcentration(
        Pollutant pollutant,
        double concentration)
    {
        _sut.SubIndex(pollutant, concentration).Should().Be(-1);
    }

    [Theory]
    [InlineData(Pollutant.Pm25, 500.5)]
    [InlineData(Pollutant.Pm10, 604.1)]
    [InlineData(Pollutant.O3, 200.1)]
    public void SubIndex_ReturnsNegativeOneAboveMaximumBreakpoint(
        Pollutant pollutant,
        double concentration)
    {
        _sut.SubIndex(pollutant, concentration).Should().Be(-1);
    }

    [Theory]
    [InlineData(0, "Bom")]
    [InlineData(50, "Bom")]
    [InlineData(51, "Moderado")]
    [InlineData(100, "Moderado")]
    [InlineData(101, "USG")]
    [InlineData(150, "USG")]
    [InlineData(151, "Insalubre")]
    [InlineData(200, "Insalubre")]
    [InlineData(201, "Muito Insalubre")]
    [InlineData(300, "Muito Insalubre")]
    [InlineData(301, "Perigoso")]
    [InlineData(500, "Perigoso")]
    public void Category_ReturnsExpectedCategory(int aqi, string expected)
    {
        _sut.Category(aqi).Should().Be(expected);
    }

    [Fact]
    public void Category_ReturnsUnknownForNegativeAqi()
    {
        _sut.Category(-1).Should().Be("desconhecido");
    }

    [Fact]
    public void OverallAqi_ReturnsNegativeOneForNullReadings()
    {
        _sut.OverallAqi(null!).Should().Be(-1);
    }

    [Fact]
    public void OverallAqi_ReturnsNegativeOneForEmptyReadings()
    {
        _sut.OverallAqi(Array.Empty<Reading>()).Should().Be(-1);
    }

    [Fact]
    public void OverallAqi_IgnoresInvalidReadingsAndReturnsHighestValidIndex()
    {
        Reading[] readings =
        [
            new(Pollutant.Pm25, -0.1),
            new(Pollutant.Pm10, 80),
            new(Pollutant.O3, 201)
        ];

        _sut.OverallAqi(readings).Should().Be(64);
    }

    [Fact]
    public void OverallAqi_ReturnsNegativeOneWhenAllReadingsAreInvalid()
    {
        Reading[] readings =
        [
            new(Pollutant.Pm25, -1),
            new(Pollutant.Pm10, 605),
            new(Pollutant.O3, 201)
        ];

        _sut.OverallAqi(readings).Should().Be(-1);
    }

    [Fact]
    public void DominantPollutant_ReturnsNullForNullReadings()
    {
        _sut.DominantPollutant(null!).Should().BeNull();
    }

    [Fact]
    public void DominantPollutant_ReturnsNullForEmptyReadings()
    {
        _sut.DominantPollutant(Array.Empty<Reading>()).Should().BeNull();
    }

    [Fact]
    public void DominantPollutant_ReturnsNullWhenAllReadingsAreInvalid()
    {
        Reading[] readings =
        [
            new(Pollutant.Pm25, -1),
            new(Pollutant.Pm10, 605),
            new(Pollutant.O3, 201)
        ];

        _sut.DominantPollutant(readings).Should().BeNull();
    }

    [Fact]
    public void DominantPollutant_PrefersPm25WhenValidSubIndicesAreTied()
    {
        Reading[] readings =
        [
            new(Pollutant.O3, 54),
            new(Pollutant.Pm10, 54),
            new(Pollutant.Pm25, 12)
        ];

        _sut.DominantPollutant(readings).Should().Be(Pollutant.Pm25);
    }

    [Fact]
    public void DominantPollutant_IgnoresInvalidReadings()
    {
        Reading[] readings =
        [
            new(Pollutant.Pm25, 12),
            new(Pollutant.Pm10, 80),
            new(Pollutant.O3, 201)
        ];

        _sut.DominantPollutant(readings).Should().Be(Pollutant.Pm10);
    }

    [Fact]
    public void NowCast_ReturnsExpectedIndexForOfficialExample()
    {
        double?[] recentHours = [20.0, 10.0, 30.0];

        _sut.NowCast(Pollutant.Pm25, recentHours).Should().Be(65);
    }

    [Fact]
    public void NowCast_ReturnsNegativeOneWhenFewerThanTwoOfRecentThreeHoursArePresent()
    {
        double?[] recentHours = [20.0, null, null, 15.0];

        _sut.NowCast(Pollutant.Pm25, recentHours).Should().Be(-1);
    }

    [Fact]
    public void NowCast_ReturnsNegativeOneForNullReadings()
    {
        _sut.NowCast(Pollutant.Pm25, null!).Should().Be(-1);
    }

    [Fact]
    public void NowCast_ReturnsNegativeOneForEmptyReadings()
    {
        _sut.NowCast(Pollutant.Pm25, Array.Empty<double?>()).Should().Be(-1);
    }

    [Fact]
    public void NowCast_IgnoresReadingsAfterTheMostRecentTwelveHours()
    {
        double?[] recentHours =
        [
            20.0, 10.0, 30.0, null, null, null,
            null, null, null, null, null, null,
            500.0
        ];

        _sut.NowCast(Pollutant.Pm25, recentHours).Should().Be(65);
    }
}
