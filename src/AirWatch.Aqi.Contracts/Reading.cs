namespace AirWatch.Aqi.Contracts;

public sealed record Reading(Pollutant Pollutant, double Concentration);
