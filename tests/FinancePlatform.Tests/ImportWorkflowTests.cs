using System.Net;
using FinancePlatform.Controllers;
using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FinancePlatform.Tests;

public class ImportWorkflowTests
{
    [Fact]
    public async Task FetchDoesNotSaveAndSaveDoesNotFetchAgain()
    {
        var handler = new ProviderStub();
        var repository = new RecordingRepository();
        var controller = Create(handler, repository);
        var response = Assert.IsType<ViewResult>(await controller.SubmitStock(new()
        { Ticker = "AAPL", Date = new(2026, 9, 10) }, default));
        var model = Assert.IsType<MarketImportViewModel>(response.Model);
        Assert.NotNull(model.Token);
        Assert.Equal(1, handler.Requests);
        Assert.Null(repository.Saved);

        var savedResponse = Assert.IsType<ViewResult>(await controller.AddToDatabase(model.Token, default));
        Assert.NotNull(Assert.IsType<MarketImportViewModel>(savedResponse.Model).Message);
        Assert.Equal(1, handler.Requests);
        Assert.Equal(model.Preview, repository.Saved);
        Assert.Equal(new DateOnly(2026, 9, 10), repository.Saved!.Date);
    }

    [Fact]
    public async Task InvalidPreviewNeverReachesDatabase()
    {
        var repository = new RecordingRepository();
        var controller = Create(new ProviderStub(), repository);
        await controller.AddToDatabase("tampered-preview", default);
        Assert.False(controller.ModelState.IsValid);
        Assert.Null(repository.Saved);
    }

    private static StockController Create(ProviderStub handler, RecordingRepository repository)
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        { ["ApiKeys:Polygon"] = "test-key-never-sent-to-network" }).Build();
        return new(new MarketApiClient(new HttpClient(handler), config), repository,
            new ImportPreviewProtector(new EphemeralDataProtectionProvider()), NullLogger<StockController>.Instance);
    }

    private sealed class ProviderStub : HttpMessageHandler
    {
        public int Requests { get; private set; }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests++;
            Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
            Assert.DoesNotContain("test-key", request.RequestUri!.ToString());
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent("""{"results":[{"c":123.45}]}""") });
        }
    }

    private sealed class RecordingRepository : IMarketRepository
    {
        public ImportPreview? Saved { get; private set; }
        public Task<IReadOnlyList<StockQuote>> GetQuotesAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<IReadOnlyList<Dividend>> GetDividendsAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task SaveAsync(ImportPreview preview, CancellationToken cancellationToken = default)
        { Saved = preview; return Task.CompletedTask; }
    }
}
