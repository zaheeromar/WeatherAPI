using System.Net.NetworkInformation;
using WeatherAPI.Models;
using static WeatherAPI.Models.Enums;

namespace WeatherAPI.Services
{
    public static class WeatherHelper
    {
        public static async Task<bool> IsValidCoordinates(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }

        public static async Task<OpenWeatherData> ConvertTemperature(OpenWeatherData weatherData, string tempUnit)
        {
            switch (tempUnit.ToLower())
            {
                case nameof(TemperatureUnit.celsius):
                    weatherData.main.temp = ToCelsius(weatherData.main.temp);
                    weatherData.main.feels_like = ToCelsius(weatherData.main.feels_like);
                    weatherData.main.temp_min = ToCelsius(weatherData.main.temp_min);
                    weatherData.main.temp_max = ToCelsius(weatherData.main.temp_max);
                    weatherData.main.unit = TemperatureUnit.celsius.ToString();
                    break;

                case nameof(TemperatureUnit.fahrenheit):
                    weatherData.main.temp = ToFahrenheit(weatherData.main.temp);
                    weatherData.main.feels_like = ToFahrenheit(weatherData.main.feels_like);
                    weatherData.main.temp_min = ToFahrenheit(weatherData.main.temp_min);
                    weatherData.main.temp_max = ToFahrenheit(weatherData.main.temp_max);
                    weatherData.main.unit = TemperatureUnit.fahrenheit.ToString();
                    break;

                default:
                    throw new ArgumentException("Invalid temperature unit. Supported units are 'kelvin', 'celsius', and 'fahrenheit'.");
            }

            return weatherData;
        }

        public static async Task<TemperatureData> GetTemperatureData(List<WeatherData> weatherData, string period, string unit)
        {
            if (period.ToLower() == nameof(ForecastPeriod.week))
            {
                weatherData = weatherData.Take(7).ToList();
            }

            TemperatureData tempData = new TemperatureData
            {
                TempUnit = TemperatureUnit.kelvin.ToString(),
                AvgTemp = Math.Round(weatherData.Select(data => data.Temperature).Average()),
                LowestTemp = weatherData.Min(w => w.MinTemperature),
                HighestTemp = weatherData.Max(w => w.MaxTemperature)
            };

            if (unit.ToLower() != nameof(TemperatureUnit.kelvin))
            {
                switch (unit.ToLower())
                {
                    case nameof(TemperatureUnit.celsius):
                        tempData.TempUnit = TemperatureUnit.celsius.ToString();
                        tempData.AvgTemp = ToCelsius(tempData.AvgTemp);
                        tempData.LowestTemp = ToCelsius(tempData.LowestTemp);
                        tempData.HighestTemp = ToCelsius(tempData.HighestTemp);
                        break;

                    case nameof(TemperatureUnit.fahrenheit):
                        tempData.TempUnit = TemperatureUnit.fahrenheit.ToString();
                        tempData.AvgTemp = ToFahrenheit(tempData.AvgTemp);
                        tempData.LowestTemp = ToFahrenheit(tempData.LowestTemp);
                        tempData.HighestTemp = ToFahrenheit(tempData.HighestTemp);
                        break;

                    default:
                        throw new ArgumentException("Invalid temperature unit. Supported units are 'kelvin', 'celsius', and 'fahrenheit'.");
                }
            }
            
            return tempData;
        }

        private static double ToCelsius(double temp)
        {
            return Math.Round(temp - 273.15, 2);
        }

        private static double ToFahrenheit(double temp)
        {
            return Math.Round(temp * 9 / 5 - 459.67, 2);
        }
    }
}
