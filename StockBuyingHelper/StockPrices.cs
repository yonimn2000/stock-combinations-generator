using System.Collections.Generic;
using System.Linq;
using YahooFinanceApi;

namespace YonatanMankovich.StockBuyingHelper
{
    public class StockPrices
    {
        private IReadOnlyDictionary<string, decimal> Prices { get; set; }

        public StockPrices(params string[] symbols) => Prices = symbols.ToDictionary(k => k, v => -1m);

        public void UpdatePrices() 
            => Prices = Yahoo.Symbols(Prices.Keys.ToArray()).Fields(Field.RegularMarketPrice).QueryAsync().Result
                .ToDictionary(x => x.Key, y => (decimal)y.Value.RegularMarketPrice);

        public decimal this[string symbol] => Prices[symbol.ToUpper()];
    }
}