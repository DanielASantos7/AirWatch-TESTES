using AirWatch.Aqi.Contracts;

namespace AirWatch.Aqi.Core;

public sealed class AirQualityIndexService : IAirQualityIndex
{
    public int SubIndex(Pollutant pollutant, double concentration) =>
        throw new NotImplementedException();

    public string Category(int aqi) =>
        throw new NotImplementedException();

    public int OverallAqi(IReadOnlyList<Reading> readings) =>
        throw new NotImplementedException();

    public Pollutant? DominantPollutant(IReadOnlyList<Reading> readings) =>
        throw new NotImplementedException();

    public int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours) =>
        throw new NotImplementedException();
}
