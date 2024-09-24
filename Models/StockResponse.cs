using System.Collections.Generic;

namespace FinancePlatform.Models
{
    public class StockResponse
    {
        public string Ticker { get; set; }
        public int QueryCount { get; set; }
        public int ResultsCount { get; set; }
        public bool Adjusted { get; set; }
        public List<Result> Results { get; set; }
        public string Status { get; set; }
        public string RequestId { get; set; }
        public int Count { get; set; }
    }

    public class Result
    {
        public double V { get; set; }
        public double Vw { get; set; }
        public double O { get; set; }
        public double C { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public long T { get; set; }
        public int N { get; set; }
    }
}
