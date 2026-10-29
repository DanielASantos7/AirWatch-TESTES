using AirWatch.Aqi.Contracts;

namespace AirWatch.Aqi.Core;

public sealed class AirQualityIndexService : IAirQualityIndex
{
    private static readonly Breakpoint[] Pm25Breakpoints =
    [
        new(0.0, 12.0, 0, 50),
        new(12.0, 35.4, 51, 100),
        new(35.4, 55.4, 101, 150),
        new(55.4, 150.4, 151, 200),
        new(150.4, 250.4, 201, 300),
        new(250.4, 500.4, 301, 500)
    ];

    private static readonly Breakpoint[] Pm10Breakpoints =
    [
        new(0, 54, 0, 50),
        new(54, 154, 51, 100),
        new(154, 254, 101, 150),
        new(254, 354, 151, 200),
        new(354, 424, 201, 300),
        new(424, 604, 301, 500)
    ];

    private static readonly Breakpoint[] O3Breakpoints =
    [
        new(0, 54, 0, 50),
        new(54, 70, 51, 100),
        new(70, 85, 101, 150),
        new(85, 105, 151, 200),
        new(105, 200, 201, 300)
    ];

    public int SubIndex(Pollutant pollutant, double concentration)
    {
        var (maximum, breakpoints, decimalPlaces) = pollutant switch
        {
            Pollutant.Pm25 => (500.4, Pm25Breakpoints, 1),
            Pollutant.Pm10 => (604.0, Pm10Breakpoints, 0),
            Pollutant.O3 => (200.0, O3Breakpoints, 0),
            _ => (double.NaN, Array.Empty<Breakpoint>(), 0)
        };

        if (!double.IsFinite(concentration) || concentration < 0 || concentration > maximum)
        {
            return -1;
        }

        var scale = Math.Pow(10, decimalPlaces);
        var truncatedConcentration = Math.Truncate(concentration * scale) / scale;

        foreach (var breakpoint in breakpoints)
        {
            if (truncatedConcentration <= breakpoint.ConcentrationHigh)
            {
                var index =
                    ((breakpoint.IndexHigh - breakpoint.IndexLow) /
                     (breakpoint.ConcentrationHigh - breakpoint.ConcentrationLow)) *
                    (truncatedConcentration - breakpoint.ConcentrationLow) +
                    breakpoint.IndexLow;

                return (int)Math.Round(index, MidpointRounding.AwayFromZero);
            }
        }

        return -1;
    }

    public string Category(int aqi) =>
        aqi switch
        {
            < 0 => "desconhecido",
            <= 50 => "Bom",
            <= 100 => "Moderado",
            <= 150 => "USG",
            <= 200 => "Insalubre",
            <= 300 => "Muito Insalubre",
            _ => "Perigoso"
        };

    public int OverallAqi(IReadOnlyList<Reading> readings)
    {
        if (readings is null || readings.Count == 0)
        {
            return -1;
        }

        var highestIndex = -1;

        foreach (var reading in readings)
        {
            var subIndex = SubIndex(reading.Pollutant, reading.Concentration);
            if (subIndex > highestIndex)
            {
                highestIndex = subIndex;
            }
        }

        return highestIndex;
    }

    public Pollutant? DominantPollutant(IReadOnlyList<Reading> readings)
    {
        if (readings is null || readings.Count == 0)
        {
            return null;
        }

        var highestIndex = -1;
        Pollutant? dominantPollutant = null;

        foreach (var reading in readings)
        {
            var subIndex = SubIndex(reading.Pollutant, reading.Concentration);
            if (subIndex > highestIndex ||
                (subIndex == highestIndex && subIndex >= 0 &&
                 (dominantPollutant is null || reading.Pollutant < dominantPollutant.Value)))
            {
                highestIndex = subIndex;
                dominantPollutant = reading.Pollutant;
            }
        }

        return dominantPollutant;
    }

    public int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours)
    {
        if (recentHours is null || recentHours.Count == 0)
        {
            return -1;
        }

        var hoursToProcess = Math.Min(recentHours.Count, 12);
        var recentPresentHours = 0;
        for (var i = 0; i < Math.Min(hoursToProcess, 3); i++)
        {
            if (recentHours[i].HasValue)
            {
                recentPresentHours++;
            }
        }

        if (recentPresentHours < 2)
        {
            return -1;
        }

        var minimum = double.PositiveInfinity;
        var maximum = double.NegativeInfinity;
        for (var i = 0; i < hoursToProcess; i++)
        {
            if (recentHours[i] is not double concentration)
            {
                continue;
            }

            minimum = Math.Min(minimum, concentration);
            maximum = Math.Max(maximum, concentration);
        }

        var weight = maximum == 0
            ? 1.0
            : Math.Clamp(minimum / maximum, 0.5, 1.0);

        var weightedConcentrationSum = 0.0;
        var weightSum = 0.0;
        for (var i = 0; i < hoursToProcess; i++)
        {
            if (recentHours[i] is not double concentration)
            {
                continue;
            }

            var hourWeight = Math.Pow(weight, i);
            weightedConcentrationSum += hourWeight * concentration;
            weightSum += hourWeight;
        }

        if (weightSum == 0 || !double.IsFinite(weightedConcentrationSum))
        {
            return -1;
        }

        return SubIndex(pollutant, weightedConcentrationSum / weightSum);
    }

    private readonly record struct Breakpoint(
        double ConcentrationLow,
        double ConcentrationHigh,
        int IndexLow,
        int IndexHigh);
}
