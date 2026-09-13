using FinancePlatform.Models;
using Npgsql;

namespace FinancePlatform.Services;

// Connections are opened only by explicit reads/saves, never by construction or startup.
public sealed class PostgresMarketRepository(IConfiguration configuration) : IMarketRepository
{
    private async Task<NpgsqlConnection> OpenAsync(CancellationToken ct)
    {
        var configured = configuration.GetConnectionString("Finance");
        if (string.IsNullOrWhiteSpace(configured))
            throw new InvalidOperationException("Database is not configured.");
        var settings = new NpgsqlConnectionStringBuilder(configured) { Timeout = 3, CommandTimeout = 5 };
        var connection = new NpgsqlConnection(settings.ConnectionString);
        try { await connection.OpenAsync(ct); return connection; }
        catch { await connection.DisposeAsync(); throw; }
    }

    public async Task<IReadOnlyList<StockQuote>> GetQuotesAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT DISTINCT ON (ticker) ticker, close_price, price_date FROM stock_prices ORDER BY ticker, price_date DESC", connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<StockQuote>();
        while (await reader.ReadAsync(cancellationToken))
            result.Add(new(reader.GetString(0), reader.GetString(0), reader.GetDecimal(1), reader.GetFieldValue<DateOnly>(2)));
        return result;
    }

    public async Task<IReadOnlyList<Dividend>> GetDividendsAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT ticker, amount, frequency, ex_date, pay_date FROM dividends ORDER BY ex_date DESC, ticker", connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Dividend>();
        while (await reader.ReadAsync(cancellationToken))
            result.Add(new(reader.GetString(0), reader.GetString(0), reader.GetDecimal(1),
                (DividendFrequency)reader.GetInt32(2), reader.GetFieldValue<DateOnly>(3), reader.GetFieldValue<DateOnly>(4)));
        return result;
    }

    public async Task SaveAsync(ImportPreview preview, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        if (preview.ClosePrice is { } price)
        {
            await using var command = new NpgsqlCommand("INSERT INTO stock_prices (ticker, price_date, close_price) VALUES (@ticker, @date, @price) ON CONFLICT (ticker, price_date) DO UPDATE SET close_price = EXCLUDED.close_price, imported_at = now()", connection, transaction);
            command.Parameters.AddWithValue("ticker", preview.Ticker);
            command.Parameters.AddWithValue("date", preview.Date);
            command.Parameters.AddWithValue("price", price);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        if (preview.Dividend is { } dividend)
        {
            await using var command = new NpgsqlCommand("INSERT INTO dividends (provider_id, ticker, amount, frequency, ex_date, pay_date) VALUES (@id, @ticker, @amount, @frequency, @ex, @pay) ON CONFLICT (provider_id) DO UPDATE SET ticker = EXCLUDED.ticker, amount = EXCLUDED.amount, frequency = EXCLUDED.frequency, ex_date = EXCLUDED.ex_date, pay_date = EXCLUDED.pay_date, imported_at = now()", connection, transaction);
            command.Parameters.AddWithValue("id", preview.DividendId!);
            command.Parameters.AddWithValue("ticker", dividend.Ticker);
            command.Parameters.AddWithValue("amount", dividend.AmountPerShare);
            command.Parameters.AddWithValue("frequency", (int)dividend.Frequency);
            command.Parameters.AddWithValue("ex", dividend.ExDividendDate);
            command.Parameters.AddWithValue("pay", dividend.PaymentDate);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        await transaction.CommitAsync(cancellationToken);
    }
}
