using System.Security.Cryptography;
using System.Text.Json;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace FinancePlatform.Controllers;

public class StockController(MarketApiClient api, IMarketRepository repository,
    ImportPreviewProtector previews, ILogger<StockController> logger) : Controller
{
    [HttpGet]
    public IActionResult StockView() => View(new MarketImportViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitStock([Bind(Prefix = "Input")] MarketImportInput input, CancellationToken ct)
    {
        var model = new MarketImportViewModel { Input = input };
        if (input.Date > DateOnly.FromDateTime(DateTime.UtcNow))
            ModelState.AddModelError("Input.Date", "Choose today or an earlier date.");
        if (!ModelState.IsValid) return View("StockView", model);
        try
        {
            model.Preview = await api.FetchAsync(input.Ticker.ToUpperInvariant(), input.Date!.Value, input.Dividend, ct);
            model.Token = previews.Protect(model.Preview);
        }
        catch (InvalidOperationException ex) { ModelState.AddModelError("", ex.Message); }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or KeyNotFoundException or FormatException or TaskCanceledException)
        {
            ModelState.AddModelError("", "Could not retrieve valid data from the provider. Try again later.");
        }
        return View("StockView", model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToDatabase(string? token, CancellationToken ct)
    {
        var model = new MarketImportViewModel();
        if (string.IsNullOrWhiteSpace(token))
        {
            ModelState.AddModelError("", "Fetch a preview before saving.");
            return View("StockView", model);
        }
        try { model.Preview = previews.Unprotect(token); }
        catch (Exception ex) when (ex is CryptographicException or JsonException or InvalidOperationException)
        {
            ModelState.AddModelError("", "Preview expired or was changed. Fetch it again before saving.");
            return View("StockView", model);
        }
        model.Input = new() { Ticker = model.Preview.Ticker, Date = model.Preview.Date, Dividend = model.Preview.Dividend is not null };
        try
        {
            await repository.SaveAsync(model.Preview, ct);
            model.Message = "Saved successfully. Switch a data page to Database to see this record. Repeated saves update the same record.";
        }
        catch (Exception ex) when (ex is NpgsqlException or InvalidOperationException or ArgumentException or TimeoutException)
        {
            logger.LogWarning("Database save failed ({ErrorType}).", ex.GetType().Name);
            model.Token = token;
            ModelState.AddModelError("", "Save failed. Check the database configuration and connection, then retry. Your preview is valid for 20 minutes.");
        }
        return View("StockView", model);
    }
}