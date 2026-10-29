namespace AirWatch.Aqi.Contracts;

/// <summary>
/// Identifies a pollutant supported by the air quality index calculations.
/// </summary>
public enum Pollutant
{
    /// <summary>Fine particulate matter with an aerodynamic diameter of 2.5 micrometers or less.</summary>
    Pm25,

    /// <summary>Particulate matter with an aerodynamic diameter of 10 micrometers or less.</summary>
    Pm10,

    /// <summary>Ozone.</summary>
    O3
}
