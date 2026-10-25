using AirWatch.Aqi.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace AirWatch.Aqi.Core;

public static class ServiceResolver
{
    private static readonly ServiceProvider ServiceProvider = new ServiceCollection()
        .AddSingleton<IAirQualityIndex, AirQualityIndexService>()
        .BuildServiceProvider();

    public static T Resolve<T>() where T : class =>
        ServiceProvider.GetRequiredService<T>();
}
