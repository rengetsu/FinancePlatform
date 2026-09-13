namespace FinancePlatform.Models;

public sealed record DividendIncome(
    string Ticker,
    string CompanyName,
    decimal Shares,
    decimal DividendPerShare,
    int PaymentsPerYear,
    decimal IncomePerPayment,
    decimal AnnualIncome,
    decimal MonthlyAverageIncome);
