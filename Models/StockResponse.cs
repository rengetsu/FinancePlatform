using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FinancePlatform.Models
{
    public class StockResponse
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; }

        [JsonPropertyName("queryCount")]
        public int QueryCount { get; set; }

        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }

        [JsonPropertyName("adjusted")]
        public bool Adjusted { get; set; }

        [JsonPropertyName("results")]
        public List<StockResult> Results { get; set; } // List for the results array

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("request_id")]
        public string Request_Id { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
