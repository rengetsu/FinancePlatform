namespace FinancePlatform.Models;

public enum DataSource { Demo, Database }

public sealed record SourceStatus(DataSource Source, string? Error = null)
{
    public string Label => Source == DataSource.Database ? "Saved database data" : "Sample data";
    public string Description => Source == DataSource.Database
        ? "Previously imported data, not a live feed. Fetch and save records on the API page."
        : "Illustrative sample data. No database connection is made in demo mode.";
}

public sealed record DataResult<T>(IReadOnlyList<T> Items, SourceStatus Status);
