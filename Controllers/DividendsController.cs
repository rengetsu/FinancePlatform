using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class DividendsController(IDividendService dividends) : Controller
{
    public IActionResult Index() => View(new DividendCalendarViewModel(dividends.GetDividends()));
}