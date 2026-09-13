namespace FinancePlatform.Models.ViewModels;

public sealed class StockOverviewViewModel
{
    public SourceStatus SourceStatus { get; init; } = new(DataSource.Demo);
    public StockOverviewViewModel(IEnumerable<StockQuote> quotes)
    {
        Quotes = Array.AsReadOnly(quotes.OrderByDescending(quote => quote.Price).ToArray());
    }

    public IReadOnlyList<StockQuote> Quotes { get; }
    public int CompanyCount => Quotes.Count;
    public StockQuote? HighestPrice => Quotes.FirstOrDefault();
    public StockQuote? LowestPrice => Quotes.LastOrDefault();
}
