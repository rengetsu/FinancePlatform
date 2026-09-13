using FinancePlatform.Models;
using FinancePlatform.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using Xunit;

namespace FinancePlatform.Tests;

public sealed class ThrowingRepository : IMarketRepository
{
    public int Calls { get; private set; }
    public Task<IReadOnlyList<StockQuote>> GetQuotesAsync(CancellationToken cancellationToken = default)
    { Calls++; throw new InvalidOperationException("Offline"); }
    public Task<IReadOnlyList<Dividend>> GetDividendsAsync(CancellationToken cancellationToken = default)
    { Calls++; throw new InvalidOperationException("Offline"); }
    public Task SaveAsync(ImportPreview preview, CancellationToken cancellationToken = default) => throw new NotSupportedException();
}

public class MarketDataTests
{
    [Fact]
    public async Task DemoNeverTouchesRepositoryEvenWhenDatabaseIsUnavailable()
    {
        var repository = new ThrowingRepository();
        var service = Create(repository);
        Assert.Equal(15, (await service.GetQuotesAsync(DataSource.Demo)).Items.Count);
        Assert.Equal(6, (await service.GetDividendsAsync(DataSource.Demo)).Items.Count);
        Assert.Equal(15, (await service.GetQuotesAsync((DataSource)99)).Items.Count);
        Assert.Equal(0, repository.Calls);
    }

    [Fact]
    public async Task DatabaseFailureDoesNotMasqueradeAsDemoData()
    {
        var repository = new ThrowingRepository();
        var service = Create(repository);
        var result = await service.GetQuotesAsync(DataSource.Database);
        Assert.Empty(result.Items);
        Assert.Equal(DataSource.Database, result.Status.Source);
        Assert.NotNull(result.Status.Error);
        Assert.Equal(1, repository.Calls);
    }

    [Fact]
    public void PreviewCannotBeEditedByBrowser()
    {
        var protector = new ImportPreviewProtector(new EphemeralDataProtectionProvider());
        var preview = new ImportPreview("AAPL", new(2026, 9, 10), 123.45m, null, null);
        var token = protector.Protect(preview);
        Assert.Equal(preview, protector.Unprotect(token));
        Assert.Throws<CryptographicException>(() => protector.Unprotect("changed" + token));
    }

    [Fact]
    public void PriceMappingKeepsRequestedTradingDateAndDecimalPrecision()
    {
        using var json = JsonDocument.Parse("""{"results":[{"c":123.456789}]}""");
        var preview = MarketApiClient.Parse(json.RootElement, "AAPL", new(2026, 9, 10), false);
        Assert.Equal(123.456789m, preview.ClosePrice);
        Assert.Equal(new DateOnly(2026, 9, 10), preview.Date);
    }

    [Fact]
    public void DividendMappingUsesProviderIdAndPaymentDates()
    {
        using var json = JsonDocument.Parse("""{"results":[{"id":"provider-id","currency":"USD","frequency":4,"cash_amount":0.26,"ex_dividend_date":"2026-08-11","pay_date":"2026-08-14"}]}""");
        var preview = MarketApiClient.Parse(json.RootElement, "AAPL", new(2026, 9, 10), true);
        Assert.Equal("provider-id", preview.DividendId);
        Assert.Equal(DividendFrequency.Quarterly, preview.Dividend!.Frequency);
        Assert.Equal(new DateOnly(2026, 8, 14), preview.Dividend.PaymentDate);
    }

    [Fact]
    public void EmptyApiResultsCannotBeSavedAsZeroPrice()
    {
        using var json = JsonDocument.Parse("""{"results":[]}""");
        Assert.Throws<InvalidOperationException>(() => MarketApiClient.Parse(json.RootElement, "AAPL", new(2026, 9, 10), false));
    }

    [Theory]
    [InlineData("EUR", 4)]
    [InlineData("USD", 0)]
    public void UnsupportedCurrencyAndFrequencyAreRejected(string currency, int frequency)
    {
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(new { results = new[] { new { currency, frequency } } }));
        Assert.Throws<InvalidOperationException>(() => MarketApiClient.Parse(json.RootElement, "AAPL", new(2026, 9, 10), true));
    }

    private static MarketDataService Create(IMarketRepository repository) =>
        new(new SampleStockQuoteService(), new SampleDividendService(), repository, NullLogger<MarketDataService>.Instance);
}
