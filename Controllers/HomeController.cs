using FinancePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FinancePlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly StockService _stockService;

        // Consolidated constructor
        public HomeController(StockService stockService, ILogger<HomeController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }
        

        private readonly ILogger<HomeController> _logger;


        public IActionResult Index()
        {
            return View(new StockPrice());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> GetStockPrice(string symbol)
        {
            
            if (string.IsNullOrWhiteSpace(symbol))
            {
                ModelState.AddModelError(string.Empty, "Please enter a stock symbol.");
                return View("Index");
            }

            var stockPrice = await _stockService.GetStockPriceAsync(symbol);

            return View("Index", stockPrice);
        }
    }
}
