using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WeatherAPI.Models;
using WeatherAPI.Services;
using WeatherAPI.Services.Interfaces;
using static WeatherAPI.Models.Enums;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class WeatherController : ControllerBase
    {
        private readonly IOpenWeatherAPIService _weatherService;

        public WeatherController(IOpenWeatherAPIService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] double lat, [FromQuery] double lon, [FromQuery] string unit = nameof(TemperatureUnit.kelvin))
        {
            try
            {
                if (!await WeatherHelper.IsValidCoordinates(lat, lon))
                {
                    return BadRequest("Invalid latitude or longitude.");
                }

                var weatherData = await _weatherService.GetWeatherData(lat, lon);

                if (weatherData == null)
                {
                    return NotFound("Weather data not found.");
                }

                if (unit != nameof(TemperatureUnit.kelvin))
                {
                    weatherData = await WeatherHelper.ConvertTemperature(weatherData, unit);
                }

                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("forecast/averagetemp")]
        public async Task<IActionResult> GetWeatherForecast([FromQuery] double lat, double lon, [FromQuery] string period = nameof(ForecastPeriod.month), [FromQuery] string unit = nameof(TemperatureUnit.kelvin))
        {
            try
            {
                if (!await WeatherHelper.IsValidCoordinates(lat, lon))
                {
                    return BadRequest("Invalid latitude or longitude.");
                }

                var forecastData = await _weatherService.GetWeatherForecast(lat, lon);

                if (forecastData == null)
                {
                    return NotFound("Weather forecast data not found.");
                }

                var temeratureData = await WeatherHelper.GetTemperatureData(forecastData, period, unit);
                return Ok(temeratureData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
