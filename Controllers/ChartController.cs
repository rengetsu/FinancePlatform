using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class ChartController(MarketDataService data) : Controller
{
    public async Task<IActionResult> Chart(DataSource source = DataSource.Demo, CancellationToken ct = default)
    {
        var result = await data.GetQuotesAsync(source, ct);
        return View("Chart", new StockOverviewViewModel(result.Items) { SourceStatus = result.Status });
    }

    public Task<IActionResult> Index(DataSource source = DataSource.Demo, CancellationToken ct = default) => Chart(source, ct);
}