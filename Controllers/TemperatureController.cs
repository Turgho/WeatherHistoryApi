using Microsoft.AspNetCore.Mvc;
using WeatherHistoryApi.Services;

namespace WeatherHistoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemperatureController : Controller
{
    private readonly WeatherService  _weatherService;
    
    public TemperatureController(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTemperatureRecords([FromQuery] string cityName)
    {
        var records = await _weatherService.GetTemperatures(cityName);
        
        return Ok(new
            {
                cityName = cityName,
                data = new
                {
                    tempMax = records.MaxTemperature,
                    tempMin = records.MinTemperature,
                    tempAvg = records.AvgTemperature
                }
            });
    }

    [HttpGet("historical")]
    public async Task<IActionResult> GetHistoricalTemeperatureRecords(
        [FromQuery] string cityName,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var records = await _weatherService.GetLast3MonthsTemperatures(cityName, startDate, endDate);

        return Ok(new
            {
                cityName = cityName,
                data = new
                {
                    tempMax = records.MaxTemperature,
                    tempMin = records.MinTemperature,
                    tempAvg = records.AvgTemperature,
                    date = $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}"
                }
            });
    }
}