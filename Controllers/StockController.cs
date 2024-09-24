using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FinancePlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly StockService _stockService;

        public StockController()
        {
            _stockService = new StockService();
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetStockPrice(string symbol)
        {
            var stockPrice = await _stockService.GetStockPriceAsync(symbol);

            if (stockPrice == null)
            {
                return NotFound();
            }

            return Ok(stockPrice);
        }
    }
}
