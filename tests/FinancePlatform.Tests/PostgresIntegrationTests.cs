using FinancePlatform.Models;
using FinancePlatform.Services;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Xunit;

namespace FinancePlatform.Tests;

public sealed class PostgresFactAttribute : FactAttribute
{
    public PostgresFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FINANCE_TEST_DATABASE")))
            Skip = "Set FINANCE_TEST_DATABASE to run against an initialized PostgreSQL database.";
    }
}

public class PostgresIntegrationTests
{
    [PostgresFact]
    public async Task SavesAreIdempotentAndLatestPriceIsSelected()
    {
        var connectionString = Environment.GetEnvironmentVariable("FINANCE_TEST_DATABASE")!;
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ConnectionStrings:Finance"] = connectionString }).Build();
        var repository = new PostgresMarketRepository(configuration);
        var ticker = "TEST-" + Guid.NewGuid().ToString("N");
        var dividend = new Dividend(ticker, ticker, 0.85m, DividendFrequency.Quarterly, new(2026, 9, 1), new(2026, 9, 15));
        try
        {
            var preview = new ImportPreview(ticker, new(2026, 9, 10), 123.456789m, dividend, ticker);
            await repository.SaveAsync(preview);
            await repository.SaveAsync(preview);
            await repository.SaveAsync(preview with { Date = new(2026, 9, 9), ClosePrice = 100m, Dividend = null });
            var quotes = (await repository.GetQuotesAsync()).Where(quote => quote.Ticker == ticker).ToArray();
            Assert.Single(quotes);
            Assert.Equal(123.456789m, quotes[0].Price);
            Assert.Equal(new DateOnly(2026, 9, 10), quotes[0].PriceDate);
            Assert.Single(await repository.GetDividendsAsync(), item => item.Ticker == ticker);
            await repository.SaveAsync(preview with { ClosePrice = 150m });
            Assert.Equal(150m, (await repository.GetQuotesAsync()).Single(quote => quote.Ticker == ticker).Price);
        }
        finally
        {
            // Only this test's uniquely named records are removed.
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand("DELETE FROM stock_prices WHERE ticker=@ticker; DELETE FROM dividends WHERE ticker=@ticker", connection);
            command.Parameters.AddWithValue("ticker", ticker);
            await command.ExecuteNonQueryAsync();
        }
    }
}
