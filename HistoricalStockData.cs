using System;
using System.Linq;
using System.Threading.Tasks;
using YahooFinanceApi;

namespace FinancePlatform
{
    public class HistoricalStockData
    {
        public void getTickerRequest(string tickerName)
        {
            char continueStr = 'y';
            while (continueStr == 'y')
            {
                Console.WriteLine("Enter a stock ticker than you want historic data for: ");
                string symbol = tickerName.ToUpper();
            }
        }
    }

    class StockData
    {
        public async Task<int> getStockData(string tickerSymbol, DateTime startDate, DateTime endDate)
        {
            try
            {
                var historic_data = await Yahoo.GetHistoricalAsync(tickerSymbol, startDate, endDate);
                var security = await Yahoo.Symbols(tickerSymbol).Fields(Field.LongName).QueryAsync();
                var ticker = security[tickerSymbol];
                var companyName = ticker[Field.LongName];
                for (int i = 0; i < historic_data.Count; i++)
                {
                    Console.WriteLine(companyName + " Closing price on: " + historic_data.ElementAt(i).DateTime.Month + "/" + historic_data.ElementAt(i).DateTime.Day + "/" + historic_data.ElementAt(i).DateTime.Year + ": $" + Math.Round(historic_data.ElementAt(i).Close, 2));
                }
            }
            catch
            {

            }

            return 1;
        }
    }
}
     