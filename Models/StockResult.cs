using System.Text.Json.Serialization;

namespace FinancePlatform.Models
{
    public class StockResult
    {
        [JsonPropertyName("v")]
        public double V { get; set; } // Volume

        [JsonPropertyName("vw")]
        public double Vw { get; set; } // VWAP (Volume Weighted Average Price)

        [JsonPropertyName("o")]
        public double O { get; set; } // Open Price

        [JsonPropertyName("c")]
        public double C { get; set; } // Close Price

        [JsonPropertyName("h")]
        public double H { get; set; } // High Price

        [JsonPropertyName("l")]
        public double L { get; set; } // Low Price

        [JsonPropertyName("t")]
        public long T { get; set; } // Timestamp (Unix time)

        [JsonPropertyName("n")]
        public int N { get; set; } // Number of transactions
    }
}
