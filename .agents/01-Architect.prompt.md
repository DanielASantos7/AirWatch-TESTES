# ROLE: System Architect & Contract Specialist (.NET)

# OBJETIVO:
Gerar as interfaces de contrato e o resolvedor de injeção de dependência para a prova AirWatch (WorldSkills 2026).

# ESPECIFICAÇÕES DE CONTRATO:
1. Criar o projeto `AirWatch.Aqi.Contracts` contendo:
   - `enum Pollutant { Pm25, Pm10, O3 }`
   - `record Reading(Pollutant Pollutant, double Concentration)`
   - `interface IAirQualityIndex`:
     * int SubIndex(Pollutant pollutant, double concentration);
     * string Category(int aqi);
     * int OverallAqi(IReadOnlyList<Reading> readings);
     * Pollutant? DominantPollutant(IReadOnlyList<Reading> readings);
     * int NowCast(Pollutant pollutant, IReadOnlyList<double?> recentHours);

2. Criar o projeto `AirWatch.Aqi.Core` contendo:
   - Classe `ServiceResolver` com o método estático `Resolve<T>()` via Microsoft.Extensions.DependencyInjection.
   - Esqueleto da classe `AirQualityIndexService : IAirQualityIndex` lançando `NotImplementedException` em todos os métodos.
