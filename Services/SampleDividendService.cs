using FinancePlatform.Models;

namespace FinancePlatform.Services;

// Fictional dividend payments in USD.
public sealed class SampleDividendService : IDividendService
{
    private static readonly IReadOnlyList<Dividend> Dividends = Array.AsReadOnly(new[]
    {
        new Dividend("DEMO-A", "Amber Grove Energy", 0.85m, DividendFrequency.Quarterly, new(2026, 9, 15), new(2026, 10, 1)),
        new Dividend("DEMO-B", "Blue Harbor Foods", 0.42m, DividendFrequency.Quarterly, new(2026, 9, 18), new(2026, 10, 5)),
        new Dividend("DEMO-C", "Cedar Valley Properties", 0.24m, DividendFrequency.Monthly, new(2026, 9, 21), new(2026, 10, 8)),
        new Dividend("DEMO-D", "Dawn Ridge Technologies", 0.30m, DividendFrequency.Quarterly, new(2026, 9, 23), new(2026, 10, 12)),
        new Dividend("DEMO-E", "Elm Coast Utilities", 1.10m, DividendFrequency.Quarterly, new(2026, 9, 25), new(2026, 10, 15)),
        new Dividend("DEMO-F", "Fern Lake Industries", 1.75m, DividendFrequency.Semiannual, new(2026, 9, 28), new(2026, 10, 20))
    });

    public IReadOnlyList<Dividend> GetDividends() => Dividends;
}
