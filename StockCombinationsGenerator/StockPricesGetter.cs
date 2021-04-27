using System.Collections.Generic;
using System.Linq;
using YahooFinanceApi;

namespace YonatanMankovich.StockCombinationsGenerator
{
    /// <summary> Represents an API stock price getter for getting the prices of one or more stocks. </summary>
    public class StockPricesGetter
    {
        private IReadOnlyDictionary<string, decimal> Prices { get; set; }

        /// <summary> Creates the stock prices getter from the given symbol(s). </summary>
        /// <param name="symbols">The stock ticker symbol(s)</param>
        public StockPricesGetter(params string[] symbols) => Prices = symbols.ToDictionary(k => k, v => -1m);

        /// <summary> Updates the prices of all the stocks using the online public API. </summary>
        public void UpdatePrices() 
            => Prices = Yahoo.Symbols(Prices.Keys.ToArray()).Fields(Field.RegularMarketPrice).QueryAsync().Result
                .ToDictionary(x => x.Key, y => (decimal)y.Value.RegularMarketPrice);

        /// <summary> Gets the price of the specified stock ticker symbol. </summary>
        public decimal this[string symbol] => Prices[symbol.ToUpper()];
    }
}