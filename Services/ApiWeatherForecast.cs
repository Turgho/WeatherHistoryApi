using System.Text.Json;

namespace WeatherHistoryApi.Services;

public class ApiWeatherForecast(ILogger<ApiWeatherForecast> logger, HttpClient httpClient)
{
    public async Task<string> GetApiResponseToday(string cityName)
    {
        var coordenates = await GetCoordinatesAsync(cityName);

        if (coordenates == null) throw new Exception();
        
        var url = $"https://api.open-meteo.com/v1/forecast?" +
                  $"latitude={coordenates.Value.Latitude}&longitude={coordenates.Value.Longitude}" +
                  $"&hourly=temperature_2m";

        logger.LogInformation("Buscando previsão do dia");

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao buscar previsão do dia");

        return await response.Content.ReadAsStringAsync();

    }

    public async Task<string> GetApiResponseHistorical(string cityName, DateTime startDate, DateTime endDate)
    {
        // Limitar máximo de 3 meses
        if ((endDate - startDate).TotalDays > 90)
            throw new ArgumentException("O intervalo não pode exceder 3 meses");

        var coordenates = await GetCoordinatesAsync(cityName);

        if (coordenates == null) throw new Exception();
        
        var url = $"https://api.open-meteo.com/v1/forecast?" +
                  $"latitude={coordenates.Value.Latitude}&longitude={coordenates.Value.Longitude}" +
                  $"&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}" +
                  $"&hourly=temperature_2m";

        logger.LogInformation("Buscando histórico: {Start} - {End}", startDate, endDate);

        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Erro ao buscar histórico");

        return await response.Content.ReadAsStringAsync();
    }
    
    private async Task<(double Latitude, double Longitude)?> GetCoordinatesAsync(string cityName)
    {
        try
        {
            var url = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(cityName)}&count=1";
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            using var json = JsonDocument.Parse(content);

            var firstResult = json.RootElement.GetProperty("results")[0];

            double lat = firstResult.GetProperty("latitude").GetDouble();
            double lon = firstResult.GetProperty("longitude").GetDouble();

            return (lat, lon);
        }
        catch (Exception)
        {
            logger.LogInformation("Error finding coordinates");
            throw new Exception("Error finding coordinates");
        }
    }
}