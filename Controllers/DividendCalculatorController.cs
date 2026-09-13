using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinancePlatform.Controllers;

public class DividendCalculatorController(
    IDividendService dividends,
    DividendIncomeCalculator calculator) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(CreateViewModel(new DividendCalculatorInput()));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index([Bind(Prefix = "Input")] DividendCalculatorInput input)
    {
        var companies = dividends.GetDividends();
        var selected = companies.FirstOrDefault(dividend => dividend.Ticker == input.Ticker);
        if (!string.IsNullOrWhiteSpace(input.Ticker) && selected is null)
            ModelState.AddModelError("Input.Ticker", "Select a company from the sample list.");

        if (!ModelState.IsValid)
            return View(new DividendCalculatorViewModel { Input = input, Companies = companies });

        var result = calculator.Calculate(selected!, input.Shares!.Value);
        return View(new DividendCalculatorViewModel { Input = input, Companies = companies, Result = result });
    }

    private DividendCalculatorViewModel CreateViewModel(DividendCalculatorInput input) => new()
    {
        Input = input,
        Companies = dividends.GetDividends()
    };
}
