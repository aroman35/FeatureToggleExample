namespace FeatureToggleExample;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public record WeatherForecastV2(DateOnly Date, int TemperatureC, string? Summary, double Pressure)
    : WeatherForecast(Date, TemperatureC, Summary);