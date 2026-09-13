namespace FinancePlatform.Models.ViewModels;

public sealed class DividendCalendarViewModel
{
    public DividendCalendarViewModel(IEnumerable<Dividend> dividends)
    {
        Dividends = Array.AsReadOnly(dividends.OrderBy(dividend => dividend.ExDividendDate).ToArray());
    }

    public IReadOnlyList<Dividend> Dividends { get; }
}
