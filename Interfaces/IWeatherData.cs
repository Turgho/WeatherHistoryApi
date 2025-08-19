namespace WeatherHistoryApi.Interfaces;

public interface IWeatherData
{
    List<double> Temperatures { get; }
    List<DateTime> Hours { get; }
    double MaxTemperature { get; }
    double MinTemperature { get; }
    double AvgTemperature { get; }
}