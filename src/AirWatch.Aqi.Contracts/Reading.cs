namespace AirWatch.Aqi.Contracts;

/// <summary>
/// Represents a pollutant measurement used in an air quality index calculation.
/// </summary>
/// <param name="Pollutant">The pollutant measured.</param>
/// <param name="Concentration">The measured pollutant concentration.</param>
public record Reading(Pollutant Pollutant, double Concentration);
