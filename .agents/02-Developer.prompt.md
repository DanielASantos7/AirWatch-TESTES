# ROLE: Senior Core Logic Developer (AirWatch Domain)

# OBJETIVO:
Implementar a regra de negócio completa da interface `IAirQualityIndex` na classe `AirQualityIndexService`.

# REGRAS DA ESPECIFICAÇÃO:
1. SubIndex(pollutant, concentration):
   - Truncar concentração: PM2.5 (1 casa decimal), PM10 e O3 (inteiros).
   - Fronteira compartilhada: pertence à faixa INFERIOR.
   - Fórmula: I = round(((I_hi - I_lo) / (C_hi - C_lo)) * (C - C_lo) + I_lo).
   - Arredondamento: AwayFromZero (MidpointRounding.AwayFromZero).
   - Sentinela: Retornar -1 para concentrações negativas ou acima do breakpoint máximo do poluente.

2. Category(aqi):
   - Mapear faixas de AQI para strings exatas:
     * 0 a 50 -> "Bom"
     * 51 a 100 -> "Moderado"
     * 101 a 150 -> "USG"
     * 151 a 200 -> "Insalubre"
     * 201 a 300 -> "Muito Insalubre"
     * 301 ou mais -> "Perigoso"
     * Menor que 0 -> "desconhecido"

3. OverallAqi(readings):
   - Retorna o maior subíndice válido das leituras. Ignora leituras com subíndice -1.
   - Sentinela: Retorna -1 se readings for nulo, vazio ou sem leituras válidas.

4. DominantPollutant(readings):
   - Retorna o poluente com o maior subíndice válido.
   - Desempate: Ordem estrita do enum (Pm25 antes de Pm10 antes de O3).
   - Sentinela: Retorna null se nenhuma leitura for válida.

5. NowCast(pollutant, recentHours):
   - Considerar até 12 horas (recentHours[0] é a mais recente).
   - Suficiência de dados: Se menos de 2 das 3 horas mais recentes ([0], [1], [2]) estiverem presentes, retorne -1.
   - Calcular peso w = clamp(c_min / c_max, 0.5, 1.0). Se c_max == 0, w = 1.0.
   - Calcular média ponderada: nowcast = sum(w^i * c_i) / sum(w^i).
   - Retornar SubIndex(pollutant, nowcast) aplicando a regra de truncamento.
   - Sentinela: Retornar -1 em falha de suficiência, nulo ou vazio.
