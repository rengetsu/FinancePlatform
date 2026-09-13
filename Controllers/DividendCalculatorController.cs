using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class DividendCalculatorController(MarketDataService data, DividendIncomeCalculator calculator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(DataSource source = DataSource.Demo, CancellationToken ct = default)
    {
        var result = await data.GetDividendsAsync(source, ct);
        return View(new DividendCalculatorViewModel { Companies = Latest(result.Items), SourceStatus = result.Status });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([Bind(Prefix = "Input")] DividendCalculatorInput input,
        DataSource source = DataSource.Demo, CancellationToken ct = default)
    {
        var result = await data.GetDividendsAsync(source, ct);
        var companies = Latest(result.Items);
        var selected = companies.FirstOrDefault(dividend => dividend.Ticker == input.Ticker);
        if (!string.IsNullOrWhiteSpace(input.Ticker) && selected is null)
            ModelState.AddModelError("Input.Ticker", "Select a company from the selected data source.");
        if (result.Status.Error is not null)
            ModelState.AddModelError("", "Income cannot be calculated while the database is unavailable.");
        var income = ModelState.IsValid && selected is not null && input.Shares is > 0
            ? calculator.Calculate(selected, input.Shares.Value) : null;
        return View(new DividendCalculatorViewModel
        {
            Input = input, Companies = companies, SourceStatus = result.Status, Result = income
        });
    }

    // One recurring dividend per ticker, so historical imports are never added together.
    private static IReadOnlyList<Dividend> Latest(IReadOnlyList<Dividend> dividends) => dividends
        .GroupBy(dividend => dividend.Ticker)
        .Select(group => group.OrderByDescending(dividend => dividend.ExDividendDate).First())
        .OrderBy(dividend => dividend.Ticker).ToArray();
}