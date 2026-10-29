using AirWatch.Aqi.Contracts;
using AirWatch.Aqi.Core;

namespace AirWatch.Aqi.UnitTests;

public class AirQualityIndexTests
{
    private readonly IAirQualityIndex _sut;

    public AirQualityIndexTests()
    {
        _sut = ServiceResolver.Resolve<IAirQualityIndex>();
    }

    [Fact]
    public void SubIndex()
    {
    }

    [Fact]
    public void Category()
    {
    }

    [Fact]
    public void OverallAqi()
    {
    }

    [Fact]
    public void DominantPollutant()
    {
    }

    [Fact]
    public void NowCast()
    {
    }
}
