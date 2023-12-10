using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YonatanMankovich.StockCombinationsGenerator
{
    /// <summary> Represents an API stock price getter for getting the prices of one or more stocks. </summary>
    public class StockPricesGetter
    {
        private IReadOnlyDictionary<string, decimal> Prices { get; set; }
        private string ApiKey { get; }

        /// <summary> Creates the stock prices getter from the given symbol(s). </summary>
        /// <param name="apiKey">The <see href="https://finnhub.io"/> API Key.</param>
        /// <param name="symbols">The stock ticker symbol(s)</param>
        public StockPricesGetter(string apiKey, params string[] symbols)
        {
            ApiKey = apiKey;
            Prices = symbols.ToDictionary(k => k, v => -1m);
        }

        /// <summary> Updates the prices of all the stocks using the online public API. </summary>
        public void UpdatePrices()
        {
            Prices = GetStockPrices(Prices.Keys);
        }

        /// <summary> Gets the price of the specified stock ticker symbol. </summary>
        public decimal this[string symbol]
        {
            get
            {
                try
                {
                    return Prices[symbol.ToUpper()];
                }
                catch (KeyNotFoundException)
                {
                    throw new StockSymbolNotFoundException(symbol);
                }
            }
        }

        private IReadOnlyDictionary<string, decimal> GetStockPrices(IEnumerable<string> stockSymbols)
        {
            ConcurrentDictionary<string, decimal> stockPrices = new ConcurrentDictionary<string, decimal>();

            IEnumerable<Task> tasks = stockSymbols.Select(async stockSymbol =>
            {
                decimal price = await GetStockPrice(stockSymbol);

                if (price > 0)
                    stockPrices[stockSymbol] = price;
            });

            Task.WhenAll(tasks).Wait();

            return stockPrices;
        }

        private async Task<decimal> GetStockPrice(string stockSymbol)
        {
            using (HttpClient client = new HttpClient())
            {
                // Finnhub API endpoint for getting real-time stock quote
                string apiUrl = $"https://finnhub.io/api/v1/quote?symbol={stockSymbol}&token={ApiKey}";

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Error for {stockSymbol}: {response.StatusCode} - {response.ReasonPhrase}");

                    return ExtractValueForCurrentPrice(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception for {stockSymbol}: {ex.Message}");
                }
            }
        }

        private static decimal ExtractValueForCurrentPrice(string jsonString)
        {
            try
            {
                Match match = Regex.Match(jsonString, @"""c""\s*:\s*([\d.]+)");

                if (!match.Success)
                    throw new Exception("Key 'c' not found in the JSON string.");

                string valueString = match.Groups[1].Value;
                decimal value = decimal.Parse(valueString);

                return value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error extracting value for 'c': {ex.Message}");
            }
        }
    }
}