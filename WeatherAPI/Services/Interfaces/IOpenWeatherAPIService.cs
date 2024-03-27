using WeatherAPI.Models;

namespace WeatherAPI.Services.Interfaces
{
    public interface IOpenWeatherAPIService
    {
        Task<OpenWeatherData> GetWeatherData(double latitude, double longitude);
        Task<List<WeatherData>> GetWeatherForecast(double latitude, double longitude);
    }
}
