using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using obligatorio2024.Models;

namespace obligatorio2024.Service
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Welcome> GetWeatherAsync(string city)
        {
            var response = await _httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?q={city}&units=metric&appid=1b18b49b8d396eecb101269e9968c80d");
            return JsonConvert.DeserializeObject<Welcome>(response, Converter.Settings);
        }
    }
}
