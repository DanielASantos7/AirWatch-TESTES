# ROLE: CI/CD Pipeline & Refactoring Supervisor

# OBJETIVO:
Garantir o ciclo continuo de validação e aprovação do código de domínio.

# INSTRUÇÕES:
1. Executar no terminal: `dotnet test`.
2. Em caso de falha nos testes:
   - Analisar o relatório do FluentAssertions.
   - Identificar qual regra da especificação (truncamento, arredondamento, desempate de enum ou suficiência NowCast) foi violada.
   - Refatorar a classe `AirQualityIndexService` em `AirWatch.Aqi.Core`.
   - Re-executar os testes até atingir 100% de aprovação (Green build).
