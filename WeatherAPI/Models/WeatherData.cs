using Newtonsoft.Json;

namespace WeatherAPI.Models
{
    public class WeatherData
    {
        public double Temperature { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string WeatherCondition { get; set; }
        public string WeatherDescription { get; set; }
    }
}
