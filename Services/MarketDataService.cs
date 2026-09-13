using FinancePlatform.Models;
using Npgsql;

namespace FinancePlatform.Services;

public sealed class MarketDataService(IStockQuoteService sampleQuotes, IDividendService sampleDividends,
    IMarketRepository repository, ILogger<MarketDataService> logger)
{
    public Task<DataResult<StockQuote>> GetQuotesAsync(DataSource source, CancellationToken ct = default) =>
        ReadAsync(source, sampleQuotes.GetQuotes, repository.GetQuotesAsync, ct);

    public Task<DataResult<Dividend>> GetDividendsAsync(DataSource source, CancellationToken ct = default) =>
        ReadAsync(source, sampleDividends.GetDividends, repository.GetDividendsAsync, ct);

    private async Task<DataResult<T>> ReadAsync<T>(DataSource source, Func<IReadOnlyList<T>> demo,
        Func<CancellationToken, Task<IReadOnlyList<T>>> database, CancellationToken ct)
    {
        // Unknown values fail closed to demo; no DB operation occurs on this branch.
        if (source != DataSource.Database)
            return new(demo(), new(DataSource.Demo));
        try { return new(await database(ct), new(source)); }
        catch (Exception ex) when (ex is NpgsqlException or InvalidOperationException or ArgumentException or TimeoutException)
        {
            logger.LogWarning("Database read failed ({ErrorType}).", ex.GetType().Name);
            return new(Array.Empty<T>(), new(source, "Database unavailable. Check its configuration and that PostgreSQL is running. You can switch back to demo at any time."));
        }
    }
}
