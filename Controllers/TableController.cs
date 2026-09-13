using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class TableController(IStockQuoteService stockQuotes) : Controller
{
    public IActionResult Table() => View(new StockOverviewViewModel(stockQuotes.GetQuotes()));

    public IActionResult Index() => View("Table", new StockOverviewViewModel(stockQuotes.GetQuotes()));
}