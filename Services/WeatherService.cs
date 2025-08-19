using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WeatherHistoryApi.Models;
using WeatherHistoryApi.Utils;

namespace WeatherHistoryApi.Services;

public class WeatherService(ILogger<WeatherService> logger, IDistributedCache cache, ApiWeatherForecast api)
{
    public async Task<DayTemperatures> GetTemperatures(string cityName)
    {
        var cacheKey = $"{cityName}:{DateTime.UtcNow:yyyy-MM-dd}";

        var result = await CacheHelper.GetOrSetAsync(
            cache,
            cacheKey,
            async () =>
            {
                // JSON
                using var json = JsonDocument.Parse(await api.GetApiResponseToday(cityName));

                var hourly = json.RootElement.GetProperty("hourly");

                // List of hours
                var hours = hourly.GetProperty("time")
                    .EnumerateArray()
                    .Select(hour => hour.GetDateTime())
                    .ToList();

                // List of temperatures
                var temperatures = hourly.GetProperty("temperature_2m")
                    .EnumerateArray()
                    .Select(temperature => temperature.GetDouble())
                    .ToList();

                return new DayTemperatures(temperatures, hours);
            },
            TimeSpan.FromHours(1), 
            logger);

        return result;
    }

    public async Task<TemperatureLast3Months> GetLast3MonthsTemperatures(string cityName, DateTime startDate, DateTime endDate)
    {
        var cacheKey = $"{cityName}:{DateTime.UtcNow:yyyy-MM-dd}:{startDate:yyyy-MM-dd}:{endDate:yyyy-MM-dd}";

        var result = await CacheHelper.GetOrSetAsync(
            cache,
            cacheKey,
            async () =>
            {
                using var json = JsonDocument.Parse(await api.GetApiResponseHistorical(cityName, startDate, endDate));
                
                var hourly = json.RootElement.GetProperty("hourly");

                // List of hours
                var hours = hourly.GetProperty("time")
                    .EnumerateArray()
                    .Select(hour => hour.GetDateTime())
                    .ToList();

                // List of temperatures
                var temperatures = hourly.GetProperty("temperature_2m")
                    .EnumerateArray()
                    .Select(temperature => temperature.GetDouble())
                    .ToList();

                return new TemperatureLast3Months(startDate, endDate, temperatures, hours);
            },
            TimeSpan.FromHours(1),
            logger);
        
        return result;
    }
}