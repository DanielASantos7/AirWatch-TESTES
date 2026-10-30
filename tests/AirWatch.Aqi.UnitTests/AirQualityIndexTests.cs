using AirWatch.Aqi.Contracts;
using AirWatch.Aqi.Core;

namespace AirWatch.Aqi.UnitTests;

public class AirQualityIndexTests
{
    private readonly IAirQualityIndex _sut;

    public AirQualityIndexTests()
    {
        _sut = ServiceResolver.Resolve<IAirQualityIndex>();
    }


    [Theory]
    [InlineData(Pollutant.O3, -0.1, -1)]
    [InlineData(Pollutant.O3, 201.0, -1)]
    [InlineData(Pollutant.Pm10, -0.1, -1)]
    [InlineData(Pollutant.Pm10, 605.0, -1)]
    [InlineData(Pollutant.Pm25, -0.1, -1)]
    [InlineData(Pollutant.Pm25, 500.5, -1)]
    public void SubIndice_DeveRetornarMenosUm_QuandoConcentracaoForInvalidaOuForaDoLimite(
        Pollutant poluente,
        double concentracao,
        int resultadoEsperado
        )
    {
        int resultadoObtido = _sut.SubIndex(poluente, concentracao);
        Assert.Equal(resultadoEsperado, resultadoObtido);
    }

    [Theory]
    // EXEMPLOS RESOLVIDOS DO ENUNCIADo
    [InlineData(Pollutant.Pm25, 9.04, 38)]
    [InlineData(Pollutant.Pm25, 12.34, 52)]


    // TRUNCAMENTO PARA UNIDADES INTEIRAS (PM10 E O3)
    // DESCARTAM TODAS AS CASAS DECIMAIS
    // 53.99 -> TRUNCA PARA 53 -> CAI NA FAIXA 0-54 = SUBINDEX = 49
    [InlineData(Pollutant.Pm10, 53.99, 49)]
    [InlineData(Pollutant.O3, 53.99, 49)]

    // FRONTEIRAS COMPARTILHADAS
    // UM VALOR EM UMA FRONTEIRA COMPARTILHADA PERTENCE À FAIXA INFERIOR
    // 12. É DIVISA ENTRE FAIXA 1 E FAIXA 2, OBRIGATORIAMENTE TEM QUE RETORNAR 50
    [InlineData(Pollutant.Pm25, 0.0, 0)]
    [InlineData(Pollutant.Pm25, 12.0, 50)]
    [InlineData(Pollutant.Pm25, 35.4, 100)]

    // PONTROS EXTREMOS E TETOS VÁLIDOS DAS TABELAS DO APÊNDICE
    // TESTA OS LIMITES MÁXIMOS PERMITIDOS DE CADA POLUENTE SEM ACIONAR SENTINELA
    [InlineData(Pollutant.Pm25, 500.4, 500)]
    [InlineData(Pollutant.Pm10, 604.0, 500)]
    [InlineData(Pollutant.O3, 200.0, 300)]
    public void DeveCalcularSubIndiceCorretamente_QuandoConcentracaoForValida(
        Pollutant poluente,
        double concentracao,
        int resultadoEsperado
        )
    {
        int resultadoObtido = _sut.SubIndex(poluente, concentracao);
        Assert.Equal(resultadoEsperado, resultadoObtido);
    }


    [Theory]
    [InlineData(Pollutant.Pm25, 0.6, 3)]
    public void SubIndice_DeveRetornarSubIndiceArredondadoLongeDoZero(
        Pollutant poluente,
        double concentracao,
        int resultadoEsperado
        )
    {
        int resultadoObtido = _sut.SubIndex(poluente, concentracao);
        Assert.Equal(resultadoEsperado, resultadoObtido);
    }


    [Theory]
    [InlineData(-1, "desconhecido")]
    [InlineData(0, "Bom")]
    [InlineData(25, "Bom")]
    [InlineData(50, "Bom")]
    [InlineData(55, "Moderado")]
    [InlineData(100, "Moderado")]
    [InlineData(125, "USG")]
    [InlineData(150, "USG")]
    [InlineData(175, "Insalubre")]
    [InlineData(200, "Insalubre")]
    [InlineData(250, "Muito Insalubre")]
    [InlineData(300, "Muito Insalubre")]
    [InlineData(301, "Perigoso")]
    public void Category_DeveRetornarRotulodeAQI_QuandoValorEstiverEmFaixaDeterminada(
        int aqi,
        string rotuloEsperado
        )
    {
        string resultadoObtido = _sut.Category(aqi);

        Assert.Equal(rotuloEsperado, resultadoObtido);
    }
}
