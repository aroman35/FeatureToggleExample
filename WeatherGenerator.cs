namespace FeatureToggleExample;

public static class WeatherGenerator
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];
    
    private static readonly string[] SummariesV2 =
    [
        "Холодно", "Прохладно", "Тепло", "Жарко"
    ];

    [Obsolete("Use GenerateRandomForecastV2")]
    public static WeatherForecast GenerateRandomForecast(int index)
    {
        return new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        );
    }

    public static WeatherForecast GenerateRandomForecastV2(int index)
    {
        return new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            SummariesV2[Random.Shared.Next(SummariesV2.Length)]
        );
    }
}