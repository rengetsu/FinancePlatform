using FinancePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
        public async Task<IActionResult> SubmitStock(string stockData)
        {
            StockResponse stockInfo = await GetStockDataAsync(stockData);

            // Pass the stock information back to the view
            //ViewBag.StockInfo = stockInfo;
            //ViewBag.StockName = stockData;

           

            // Return the StockView again
            return View("StockView", stockInfo);
        }

        private async Task<StockResponse> GetStockDataAsync(string stockSymbol)
        {
            string apiKey = "9aOImHeQjnqwk8XwwkMyTdMU_5YrTw30"; // Replace with your actual API key
            string url = $"https://api.polygon.io/v2/aggs/ticker/{stockSymbol}/range/1/day/2024-09-09/2024-09-09?apiKey={apiKey}";

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
