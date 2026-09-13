using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class ChartController(IStockQuoteService stockQuotes) : Controller
{
    public IActionResult Chart() => View(new StockOverviewViewModel(stockQuotes.GetQuotes()));

    public IActionResult Index() => View("Chart", new StockOverviewViewModel(stockQuotes.GetQuotes()));
}