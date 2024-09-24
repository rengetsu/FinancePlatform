using FinancePlatform.Controllers;
using FinancePlatform.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class StockPriceModel : PageModel
{
    private readonly StockService _stockService;

    public StockPriceModel(StockService stockService)
    {
        _stockService = stockService;
    }

    [BindProperty]
    public string Symbol { get; set; }

    public StockPrice StockPrice { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        StockPrice = await _stockService.GetStockPriceAsync(Symbol);
        return Page();
    }
}
