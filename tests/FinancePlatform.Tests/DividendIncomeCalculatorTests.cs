using System.ComponentModel.DataAnnotations;
using FinancePlatform.Controllers;
using FinancePlatform.Models;
using FinancePlatform.Models.ViewModels;
using FinancePlatform.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FinancePlatform.Tests;

public class DividendIncomeCalculatorTests
{
    private readonly DividendIncomeCalculator calculator = new();

    [Theory]
    [InlineData(DividendFrequency.Monthly, 12, 1020)]
    [InlineData(DividendFrequency.Quarterly, 4, 340)]
    [InlineData(DividendFrequency.Semiannual, 2, 170)]
    [InlineData(DividendFrequency.Annual, 1, 85)]
    public void AnnualEstimateUsesPaymentFrequency(DividendFrequency frequency, int payments, int annual)
    {
        var result = calculator.Calculate(Sample(frequency), 100m);
        Assert.Equal(85m, result.IncomePerPayment);
        Assert.Equal(payments, result.PaymentsPerYear);
        Assert.Equal((decimal)annual, result.AnnualIncome);
        Assert.Equal(annual / 12m, result.MonthlyAverageIncome);
    }

    [Fact]
    public void FractionalSharesRetainPrecisionUntilDisplay()
    {
        var result = calculator.Calculate(Sample(), 12.5m);
        Assert.Equal(10.625m, result.IncomePerPayment);
        Assert.Equal(42.5m, result.AnnualIncome);
        Assert.Equal(42.5m / 12m, result.MonthlyAverageIncome);
    }

    [Fact]
    public void ZeroDividendProducesZeroIncome()
    {
        var result = calculator.Calculate(Sample() with { AmountPerShare = 0 }, 100m);
        Assert.Equal(0m, result.AnnualIncome);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void NonPositiveSharesAreRejected(int shares) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.Calculate(Sample(), shares));

    [Fact]
    public void InvalidDividendIsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => calculator.Calculate(null!, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.Calculate(Sample() with { AmountPerShare = -1 }, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => calculator.Calculate(Sample((DividendFrequency)999), 100));
    }

    [Theory]
    [InlineData(null, null, false)]
    [InlineData("DEMO-A", "0", false)]
    [InlineData("DEMO-A", "-1", false)]
    [InlineData("DEMO-A", "1000000001", false)]
    [InlineData("DEMO-A", "0.000001", true)]
    [InlineData("DEMO-A", "12.5", true)]
    public void InputValidationChecksRequiredFieldsAndShareLimits(string? ticker, string? shares, bool valid)
    {
        var input = new DividendCalculatorInput
        {
            Ticker = ticker,
            Shares = shares is null ? null : decimal.Parse(shares, System.Globalization.CultureInfo.InvariantCulture)
        };
        Assert.Equal(valid, Validator.TryValidateObject(input, new ValidationContext(input), new List<ValidationResult>(), true));
    }

    [Fact]
    public void UnknownTickerReturnsValidationErrorWithoutResult()
    {
        var controller = new DividendCalculatorController(new SampleDividendService(), calculator);
        var response = Assert.IsType<ViewResult>(controller.Index(new DividendCalculatorInput { Ticker = "UNKNOWN", Shares = 100 }));
        var model = Assert.IsType<DividendCalculatorViewModel>(response.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Null(model.Result);
        Assert.Equal(6, model.Companies.Count);
    }

    [Fact]
    public void ValidSubmissionUsesServerSideSampleAmount()
    {
        var controller = new DividendCalculatorController(new SampleDividendService(), calculator);
        var response = Assert.IsType<ViewResult>(controller.Index(new DividendCalculatorInput { Ticker = "DEMO-A", Shares = 100 }));
        var model = Assert.IsType<DividendCalculatorViewModel>(response.Model);
        Assert.Equal(340m, model.Result!.AnnualIncome);
    }

    private static Dividend Sample(DividendFrequency frequency = DividendFrequency.Quarterly) =>
        new("DEMO-A", "Sample Company", 0.85m, frequency, new(2026, 9, 15), new(2026, 10, 1));
}
