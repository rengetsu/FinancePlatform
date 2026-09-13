namespace FinancePlatform.Models;

public enum DividendFrequency
{
    Monthly,
    Quarterly,
    Semiannual,
    Annual
}

public sealed record Dividend(
    string Ticker,
    string CompanyName,
    decimal AmountPerShare,
    DividendFrequency Frequency,
    DateOnly ExDividendDate,
    DateOnly PaymentDate);
