using FinancePlatform.Models;

namespace FinancePlatform.Services;

public interface IDividendService
{
    IReadOnlyList<Dividend> GetDividends();
}
