using FinancePlatform.Models;

namespace FinancePlatform.Services;

public sealed class DividendIncomeCalculator
{
    public DividendIncome Calculate(Dividend dividend, decimal shares)
    {
        ArgumentNullException.ThrowIfNull(dividend);
        if (shares <= 0)
            throw new ArgumentOutOfRangeException(nameof(shares), "Share count must be greater than zero.");
        if (dividend.AmountPerShare < 0)
            throw new ArgumentOutOfRangeException(nameof(dividend), "Dividend amount cannot be negative.");

        var paymentsPerYear = dividend.Frequency switch
        {
            DividendFrequency.Monthly => 12,
            DividendFrequency.Quarterly => 4,
            DividendFrequency.Semiannual => 2,
            DividendFrequency.Annual => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(dividend), "Unsupported dividend frequency.")
        };

        // Keep full decimal precision. Round only when displaying currency.
        var incomePerPayment = shares * dividend.AmountPerShare;
        var annualIncome = incomePerPayment * paymentsPerYear;

        return new DividendIncome(dividend.Ticker, dividend.CompanyName, shares,
            dividend.AmountPerShare, paymentsPerYear, incomePerPayment,
            annualIncome, annualIncome / 12m);
    }
}
