namespace AirWatch.Aqi.Contracts;

/// <summary>
/// Defines operations for calculating air quality indexes from pollutant concentrations.
/// </summary>
public interface IAirQualityIndex
{
    /// <summary>
    /// Calculates the air quality sub-index for a pollutant concentration.
    /// </summary>
    /// <param name="pollutant">The pollutant whose sub-index is calculated.</param>
    /// <param name="concentration">The pollutant concentration.</param>
    /// <returns>
    /// The calculated sub-index, or -1 if the concentration is invalid or outside the supported range.
    /// </returns>
    int SubIndex(Pollutant pollutant, double concentration);

    /// <summary>
    /// Gets the category label corresponding to an air quality index.
    /// </summary>
    /// <param name="aqi">The air quality index.</param>
    /// <returns>The category label, or "desconhecido" when <paramref name="aqi"/> is negative.</returns>
    string Category(int aqi);

    /// <summary>
    /// Calculates the overall air quality index as the highest valid pollutant sub-index.
    /// </summary>
    /// <param name="readings">The pollutant readings to evaluate.</param>
    /// <returns>
    /// The highest valid sub-index, or -1 if the input is null, empty, or contains no valid readings.
    /// </returns>
    int OverallAqi(IReadOnlyList<Reading> readings);

    /// <summary>
    /// Finds the pollutant with the highest valid sub-index.
    /// </summary>
    /// <param name="readings">The pollutant readings to evaluate.</param>
    /// <returns>
    /// The dominant pollutant, using enum order to break ties, or <see langword="null"/> if the input
    /// is null, empty, or contains no valid readings.
    /// </returns>
    Pollutant? DominantPollutant(IReadOnlyList<Reading> readings);

    /// <summary>
    /// Calculates a NowCast sub-index from recent hourly concentrations.
    /// </summary>
    /// <param name="pollutant">The pollutant whose NowCast sub-index is calculated.</param>
    /// <param name="recentHours">
    /// Hourly concentrations ordered from most recent to oldest; only the first 12 entries are considered.
    /// </param>
    /// <returns>
    /// The calculated sub-index, or -1 if the input is null or empty, there are insufficient recent readings,
    /// or the resulting concentration is invalid.
    /// </returns>
    int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours);
}
