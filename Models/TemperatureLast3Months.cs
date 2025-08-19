using WeatherHistoryApi.Interfaces;

namespace WeatherHistoryApi.Models;

public class TemperatureLast3Months(
    DateTime startDate,
    DateTime endDate,
    List<double> temperatures,
    List<DateTime> hours)
    : IWeatherData
{
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;

    public List<double> Temperatures { get; } = temperatures;
    public List<DateTime> Hours { get; } = hours;
    
    public double MaxTemperature => Temperatures.Any() ? Temperatures.Max() : 0;
    public double MinTemperature => Temperatures.Any() ? Temperatures.Min() : 0;
    public double AvgTemperature => Temperatures.Any() ? Temperatures.Average() : 0;
}

