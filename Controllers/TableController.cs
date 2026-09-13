using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class TableController(MarketDataService data) : Controller
{
    public async Task<IActionResult> Table(DataSource source = DataSource.Demo, CancellationToken ct = default)
    {
        var result = await data.GetQuotesAsync(source, ct);
        return View("Table", new StockOverviewViewModel(result.Items) { SourceStatus = result.Status });
    }

    public Task<IActionResult> Index(DataSource source = DataSource.Demo, CancellationToken ct = default) => Table(source, ct);
}