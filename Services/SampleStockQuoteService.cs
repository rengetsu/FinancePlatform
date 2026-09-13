using FinancePlatform.Models;

namespace FinancePlatform.Services;

// Illustrative USD prices, shared by the chart and table. No external API calls.
public sealed class SampleStockQuoteService : IStockQuoteService
{
    private static readonly IReadOnlyList<StockQuote> Quotes = Array.AsReadOnly(new[]
    {
        new StockQuote("NFLX", "Netflix", 712.78m),
        new StockQuote("META", "Meta", 573.22m),
        new StockQuote("MSFT", "Microsoft", 418.42m),
        new StockQuote("V", "Visa Inc", 276.28m),
        new StockQuote("TSLA", "Tesla", 249.18m),
        new StockQuote("AAPL", "Apple", 226.98m),
        new StockQuote("AMZN", "Amazon", 185.38m),
        new StockQuote("NVDA", "Nvidia Corp", 118.68m),
        new StockQuote("BABA", "Alibaba", 114.32m),
        new StockQuote("NKE", "Nike", 83.07m),
        new StockQuote("PYPL", "PayPal Holdings Inc", 77.23m),
        new StockQuote("PFE", "Pfizer Inc", 28.60m),
        new StockQuote("INTC", "Intel", 22.45m),
        new StockQuote("F", "Ford", 10.48m),
        new StockQuote("NIO", "Nio Inc", 7.14m)
    });

    public IReadOnlyList<StockQuote> GetQuotes() => Quotes;
}
