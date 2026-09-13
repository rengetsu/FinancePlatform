namespace FinancePlatform.Models;

public enum DividendFrequency
{
    Monthly = 0,
    Quarterly = 1,
    Semiannual = 2,
    Annual = 3
}

public sealed record Dividend(
    string Ticker,
    string CompanyName,
    decimal AmountPerShare,
    DividendFrequency Frequency,
    DateOnly ExDividendDate,
    DateOnly PaymentDate);
