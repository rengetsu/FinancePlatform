using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using FinancePlatform.Models;

namespace FinancePlatform.Services;

public sealed class MarketApiClient(HttpClient http, IConfiguration configuration)
{
    public async Task<ImportPreview> FetchAsync(string ticker, DateOnly date, bool dividend, CancellationToken ct)
    {
        var key = configuration["ApiKeys:Polygon"];
        if (string.IsNullOrWhiteSpace(key))
            throw new InvalidOperationException("Set ApiKeys:Polygon in user secrets or POLYGON_API_KEY in Docker's .env before fetching.");
        var symbol = Uri.EscapeDataString(ticker);
        var day = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var path = dividend
            ? $"stocks/v1/dividends?ticker={symbol}&ex_dividend_date.lte={day}&distribution_type=recurring&sort=ex_dividend_date.desc&limit=1"
            : $"v2/aggs/ticker/{symbol}/range/1/day/{day}/{day}?adjusted=true";
        // Polygon is now Massive. Authentication stays in a header, not logged URLs.
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.massive.com/" + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        using var response = await http.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"The provider returned HTTP {(int)response.StatusCode}. Check API access, quota, and the selected date.");
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        return Parse(json.RootElement, ticker, date, dividend);
    }

    public static ImportPreview Parse(JsonElement root, string ticker, DateOnly date, bool dividend)
    {
        if (!root.TryGetProperty("results", out var rows) || rows.ValueKind != JsonValueKind.Array || rows.GetArrayLength() == 0)
            throw new InvalidOperationException("No data was returned. Try a trading day or a different ticker/date.");
        var row = rows[0];
        if (!dividend)
        {
            if (!row.TryGetProperty("c", out var close) || !close.TryGetDecimal(out var price) || price < 0)
                throw new InvalidOperationException("The provider did not return a valid closing price.");
            return new(ticker, date, price, null, null);
        }
        if (row.GetProperty("currency").GetString() != "USD")
            throw new InvalidOperationException("Only USD dividends are supported by this calculator.");
        var frequency = row.GetProperty("frequency").GetInt32() switch
        {
            12 => DividendFrequency.Monthly,
            4 => DividendFrequency.Quarterly,
            2 => DividendFrequency.Semiannual,
            1 => DividendFrequency.Annual,
            _ => throw new InvalidOperationException("This dividend's payment frequency is not supported by the calculator.")
        };
        var amount = row.GetProperty("cash_amount").GetDecimal();
        var id = row.GetProperty("id").GetString();
        if (amount < 0 || string.IsNullOrWhiteSpace(id) ||
            !DateOnly.TryParseExact(row.GetProperty("ex_dividend_date").GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var exDate) ||
            !DateOnly.TryParseExact(row.GetProperty("pay_date").GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var payDate))
            throw new InvalidOperationException("The provider returned incomplete dividend data; nothing was saved.");
        return new(ticker, date, null, new(ticker, ticker, amount, frequency, exDate, payDate), id);
    }
}
