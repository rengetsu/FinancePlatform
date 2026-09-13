namespace FinancePlatform.Models;

public sealed record StockQuote(string Ticker, string CompanyName, decimal Price);
