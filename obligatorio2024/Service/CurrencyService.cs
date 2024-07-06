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
            if (sourceCurrency == "UYU" && targetCurrency == "UYU")
            {
                return 1; // Si la moneda de origen y destino es UYU, el tipo de cambio es 1
            }

            var request = new RestRequest($"/live?access_key=f55f846b492d3599fcf319f0f278583c&currencies={targetCurrency}&source={sourceCurrency}", Method.Get);
            RestResponse response = await _client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                var data = JObject.Parse(response.Content);
                var quotes = data["quotes"] as JObject;

                if (quotes == null)
                {
                    throw new ArgumentException("No quotes found in the response.");
                }

                string key = $"{sourceCurrency}{targetCurrency}";
                var rate = quotes[key]?.Value<decimal>();

                if (rate.HasValue)
                {
                    return rate;
                }
                else
                {
                    throw new ArgumentException($"Exchange rate not found for {key}");
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
