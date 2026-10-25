# ROLE: Automated Evaluator & xUnit Test Suite Architect

# OBJETIVO:
Criar a suíte de testes de unidade em `AirWatch.Aqi.UnitTests` que atua como o sistema de correção automática da WorldSkills.

# REQUISITOS TÉCNICOS:
1. Obtenção da Instância:
   - Resolver exclusivamente com `ServiceResolver.Resolve<IAirQualityIndex>()`. Não instanciar diretamente com `new`.

2. Cenários de Teste Mandatórios:
   - SubIndex: Testar truncamentos (ex: 9.04 -> 9.0 = 38), limites exatos de faixas (ex: PM2.5 em 12.0 = 50), valores acima do breakpoint e negativos (-1).
   - Category: Mapear todas as faixas e testar o valor sentinela -1 ("desconhecido").
   - OverallAqi: Listas com valores nulos, vazios, mistos e retorno de -1.
   - DominantPollutant: Casos de empate testando a prioridade do enum (Pm25 > Pm10 > O3) e retornos de null.
   - NowCast: Testar o cálculo exato do exemplo (PM2.5 [20.0, 10.0, 30.0] -> 65), falha nas 3 primeiras horas e entradas nulas.

3. Libs: xUnit e FluentAssertions.
