using FinancePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinancePlatform.Controllers
{
    public class StockController : Controller
    {
        private readonly HttpClient _httpClient;

        public StockController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult StockView()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitStock(string stockData, DateTime stockTimeData)
        {
            // Format the date to 'yyyy-MM-dd'
            string formattedDate = stockTimeData.ToString("yyyy-MM-dd");

            StockResponse stockInfo = await GetStockDataAsync(stockData, formattedDate);

            // Return the StockView again
            return View("StockView", stockInfo);
        }

        private async Task<StockResponse> GetStockDataAsync(string stockSymbol, string date)
        {
            string apiKey = "9aOImHeQjnqwk8XwwkMyTdMU_5YrTw30"; // Replace with your actual API key
            string url = $"https://api.polygon.io/v2/aggs/ticker/{stockSymbol}/range/1/day/{date}/{date}?apiKey={apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonData);
                var stockResponse = JsonSerializer.Deserialize<StockResponse>(jsonData);
                return stockResponse;
            }

            return null;
        }
    }
}
