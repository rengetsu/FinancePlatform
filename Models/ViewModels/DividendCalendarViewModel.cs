namespace FinancePlatform.Models.ViewModels;

public sealed class DividendCalendarViewModel
{
    public SourceStatus SourceStatus { get; init; } = new(DataSource.Demo);
    public DividendCalendarViewModel(IEnumerable<Dividend> dividends)
    {
        Dividends = Array.AsReadOnly(dividends.OrderBy(dividend => dividend.ExDividendDate).ToArray());
    }

    public IReadOnlyList<Dividend> Dividends { get; }
}
