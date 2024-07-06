using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace obligatorio2024.Service
{
    public class CurrencyService
    {
        private readonly RestClient _client;

        public CurrencyService()
        {
            var options = new RestClientOptions("http://api.currencylayer.com")
            {
                Timeout = TimeSpan.FromMilliseconds(-1)  // Esto deshabilita el tiempo de espera
            };
            _client = new RestClient(options);
        }

        public async Task<decimal?> GetExchangeRate(string sourceCurrency, string targetCurrency)
        {
            var request = new RestRequest($"/live?access_key=f55f846b492d3599fcf319f0f278583c&currencies={targetCurrency}&source={sourceCurrency}", Method.Get);
            RestResponse response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var data = JObject.Parse(response.Content);
                var quotes = data["quotes"];

                // Formamos la clave correctamente
                string key = $"{sourceCurrency}{targetCurrency}";
                var rate = quotes?[key]?.Value<decimal>();

                // Si sourceCurrency es "UYU", retornamos el valor de la tasa
                if (sourceCurrency == "UYU")
                {
                    return rate;
                }
                // Si la clave no es correcta (ej: UYU a UYU), devolvemos 1
                if (sourceCurrency == targetCurrency)
                {
                    return 1;
                }
                // Para otras conversiones, retornamos la tasa invertida
                else if (rate.HasValue)
                {
                    return 1 / rate.Value;
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ErrorMessage}");
            }
            return null;
        }

    }
}
