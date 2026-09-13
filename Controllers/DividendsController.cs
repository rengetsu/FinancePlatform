using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class DividendsController(MarketDataService data) : Controller
{
    public async Task<IActionResult> Index(DataSource source = DataSource.Demo, CancellationToken ct = default)
    {
        var result = await data.GetDividendsAsync(source, ct);
        return View(new DividendCalendarViewModel(result.Items) { SourceStatus = result.Status });
    }
}