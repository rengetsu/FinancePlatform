using FinancePlatform.Models;

namespace FinancePlatform.Services;

public interface IMarketRepository
{
    Task<IReadOnlyList<StockQuote>> GetQuotesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Dividend>> GetDividendsAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(ImportPreview preview, CancellationToken cancellationToken = default);
}
