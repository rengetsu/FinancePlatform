namespace FinancePlatform.Models
{
    public class StockEntity
    {
        public int Id { get; set; }
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double Volume { get; set; }
        public double OpenPrice { get; set; }
        public double ClosePrice { get; set; }
        // Add other relevant fields
    }
}
