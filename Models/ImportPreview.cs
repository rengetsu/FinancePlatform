namespace FinancePlatform.Models;

public sealed record ImportPreview(string Ticker, DateOnly Date, decimal? ClosePrice,
    Dividend? Dividend, string? DividendId);
