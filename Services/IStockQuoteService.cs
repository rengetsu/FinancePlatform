using FinancePlatform.Models;

namespace FinancePlatform.Services;

public interface IStockQuoteService
{
    IReadOnlyList<StockQuote> GetQuotes();
}
