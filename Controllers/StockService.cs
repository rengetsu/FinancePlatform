using System;
using System.Net.Http;
using System.Threading.Tasks;
using FinancePlatform.Models;
using Newtonsoft.Json;

namespace FinancePlatform.Controllers
{
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "9aOImHeQjnqwk8XwwkMyTdMU_5YrTw30"; // Replace with your API key

        public StockService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<StockPrice> GetStockPriceAsync(string symbol)
        {
            var url = $"https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2024-09-09/2024-09-09?apiKey=9aOImHeQjnqwk8XwwkMyTdMU_5YrTw30";
            try
            {
                var response = await _httpClient.GetStringAsync(url);

                // Deserialize the JSON response into the StockResponse model
                var stockResponse = JsonConvert.DeserializeObject<StockResponse>(response);

                var stockData = JsonConvert.DeserializeObject<StockPrice>(response);

                // Extract the closing price as a string
                var returnedValue = stockResponse.Results[0].C;

                return stockData;

            }
            //return stockData
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }
    }
}