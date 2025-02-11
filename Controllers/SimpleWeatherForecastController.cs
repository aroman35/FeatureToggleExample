using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace FeatureToggleExample.Controllers;

[ApiController]
[Route("[controller]")]
public class SimpleWeatherForecastController(IVariantFeatureManager featureManager, ILogger<SimpleWeatherForecastController> logger)
    : ControllerBase
{
    [HttpGet("[action]")]
    public async IAsyncEnumerable<WeatherForecast> Get([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var isV2Enabled = await featureManager.IsEnabledAsync(FEATURE_WEATHER_V2, cancellationToken);
        logger.LogInformation("Generating {Version} response", isV2Enabled ? "v2" : "v1");
        foreach (var index in Enumerable.Range(1, 5))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return isV2Enabled
                ? WeatherGenerator.GenerateRandomForecastV2(index)
                : WeatherGenerator.GenerateRandomForecast(index);
        }
    }
}