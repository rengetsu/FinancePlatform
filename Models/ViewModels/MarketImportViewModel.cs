using System.ComponentModel.DataAnnotations;

namespace FinancePlatform.Models.ViewModels;

public sealed class MarketImportInput
{
    [Required, RegularExpression(@"[A-Za-z0-9.\-]{1,15}", ErrorMessage = "Enter a valid ticker (up to 15 letters, numbers, dots or hyphens).")]
    public string Ticker { get; set; } = "";
    [Required]
    public DateOnly? Date { get; set; }
    public bool Dividend { get; set; }
}

public sealed class MarketImportViewModel
{
    public MarketImportInput Input { get; set; } = new() { Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)) };
    public ImportPreview? Preview { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
}
