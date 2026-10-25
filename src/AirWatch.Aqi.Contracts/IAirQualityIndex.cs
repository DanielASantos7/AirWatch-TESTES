namespace AirWatch.Aqi.Contracts;

public interface IAirQualityIndex
{
    int SubIndex(Pollutant pollutant, double concentration);

    string Category(int aqi);

    int OverallAqi(IReadOnlyList<Reading> readings);

    Pollutant? DominantPollutant(IReadOnlyList<Reading> readings);

    int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours);
}
