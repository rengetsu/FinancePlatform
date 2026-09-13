using System.ComponentModel.DataAnnotations;

namespace FinancePlatform.Models.ViewModels;

public sealed class DividendCalculatorInput
{
    [Required(ErrorMessage = "Select a sample company.")]
    [Display(Name = "Sample company")]
    public string? Ticker { get; set; }

    [Required(ErrorMessage = "Enter the number of shares.")]
    [Range(typeof(decimal), "0.000001", "1000000000",
        ErrorMessage = "Enter between 0.000001 and 1,000,000,000 shares.")]
    [Display(Name = "Number of shares")]
    public decimal? Shares { get; set; }
}

public sealed class DividendCalculatorViewModel
{
    public DividendCalculatorInput Input { get; init; } = new();
    public IReadOnlyList<Dividend> Companies { get; init; } = Array.Empty<Dividend>();
    public DividendIncome? Result { get; init; }
}
