using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WeatherAPI.Models;
using WeatherAPI.Services.Interfaces;

namespace WeatherAPI.Services
{
    public class OpenWeatherAPIService : IOpenWeatherAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthConfiguration _auth;

        public OpenWeatherAPIService(HttpClient httpClient, IOptions<AuthConfiguration> auth)
        {
            _httpClient = httpClient;
            _auth = auth.Value;
        }

        public async Task<OpenWeatherData> GetWeatherData(double latitude, double longitude)
        {
            try
            {
                string apiUrl = $"{_httpClient.BaseAddress}/weather?lat={latitude}&lon={longitude}&appid={_auth.OpenWeatherAPIKey}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<OpenWeatherData>(jsonResponse);

                    if (weatherData != null)
                    {
                        weatherData.main.unit = Enums.TemperatureUnit.kelvin.ToString();
                    }

                    return weatherData;
                }
                else
                {
                    throw new HttpRequestException($"Failed to get weather data. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting weather data.", ex);
            }
        }

        public async Task<List<WeatherData>> GetWeatherForecast(double latitude, double longitude)
        {
            try
            {
                string apiUrl = $"{_httpClient.BaseAddress}/forecast?lat={latitude}&lon={longitude}&appid={_auth.OpenWeatherAPIKey}";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);

                    List<WeatherData> weatherForecast = new List<WeatherData>();

                    foreach (var item in responseObject.list)
                    {
                        var weatherData = new WeatherData
                        {
                            Temperature = (double)item.main.temp,
                            Humidity = (double)item.main.humidity,
                            WindSpeed = (double)item.wind.speed,
                            MinTemperature = (double)item.main.temp_min,
                            MaxTemperature = (double)item.main.temp_max,
                            WeatherCondition = (string)item.weather[0].main,
                            WeatherDescription = (string)item.weather[0].description
                        };

                        weatherForecast.Add(weatherData);
                    }

                    return weatherForecast;
                }
                else
                {
                    throw new HttpRequestException($"Failed to get weather forecast. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting weather forecast.", ex);
            }
        }
    }
}
