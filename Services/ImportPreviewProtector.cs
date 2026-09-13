using System.Text.Json;
using FinancePlatform.Models;
using Microsoft.AspNetCore.DataProtection;

namespace FinancePlatform.Services;

// Authenticated, expiring payload: the browser cannot alter prices/dates before saving.
public sealed class ImportPreviewProtector(IDataProtectionProvider provider)
{
    private readonly ITimeLimitedDataProtector protector = provider.CreateProtector("MarketImport.v1").ToTimeLimitedDataProtector();
    public string Protect(ImportPreview preview) => protector.Protect(JsonSerializer.Serialize(preview), TimeSpan.FromMinutes(20));
    public ImportPreview Unprotect(string token) => JsonSerializer.Deserialize<ImportPreview>(protector.Unprotect(token))
        ?? throw new InvalidOperationException("Invalid preview.");
}
