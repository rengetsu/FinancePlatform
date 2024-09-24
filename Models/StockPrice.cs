namespace FinancePlatform.Models
{
    public class StockPrice
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public long Timestamp { get; set; }
    }

}
