using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YonatanMankovich.StockCombinationsGenerator
{
    /// <summary> Represents a stock quantity combinations generator. </summary>
    public class CombinationsGenerator
    {
        /// <summary> Gets the number of possible stock quantity combinations. </summary>
        public ulong NumberOfPossibleCombinations { get; }

        /// <summary> Returns true if there are any stock quantity combinations. </summary>
        public bool HasCombinations => Combinations.Any();

        /// <summary> Gets the <see cref="Stock"/> objects that were added to the generator. </summary>
        public IEnumerable<Stock> Stocks => MaxStockQuantities.Select(s => s.Stock);

        /// <summary> Gets the max quantities of stock that one can buy with the available amount of cash. </summary>
        public StockQuantity[] MaxStockQuantities { get; }

        /// <summary> Gets the available amount of cash. </summary>
        public decimal Cash { get; }

        private IList<StockQuantity[]> Combinations { get; } = new List<StockQuantity[]>();
        private decimal BottomCashLimit { get; }
        private StockPricesGetter StockPricesGetter { get; }

        /// <summary> Creates the stock quantity combinations generator. </summary>
        /// <param name="apiKey">The <see href="https://finnhub.io"/> API Key.</param>
        /// <param name="stockSymbols"> A list of stock ticker symbols. </param>
        /// <param name="cash"> The amount of cash available for trade. </param>
        public CombinationsGenerator(string apiKey, IEnumerable<string> stockSymbols, decimal cash)
        {
            StockPricesGetter = new StockPricesGetter(apiKey, stockSymbols.Distinct().ToArray());
            StockPricesGetter.UpdatePrices();
            MaxStockQuantities = stockSymbols.Select(symbol => new StockQuantity
            {
                Stock = new Stock
                {
                    Symbol = symbol,
                    Price = StockPricesGetter[symbol]
                },
                Quantity = (uint)Math.Floor(cash / StockPricesGetter[symbol])
            }).ToArray();

            NumberOfPossibleCombinations = GetCalculatedNumberOfPossibleCombinations();
            Cash = cash;
            BottomCashLimit = GetBottomCashLimit();
        }

        private ulong GetCalculatedNumberOfPossibleCombinations()
        {
            ulong multi = 1;
            foreach (StockQuantity stockQuantity in MaxStockQuantities)
                multi *= stockQuantity.Quantity + 1;
            return multi;
        }

        // Having a high lower bound helps keep memory usage low.
        // If there are more than 100K combinations, get a multiplier of 0.9, 0.99, 0.999, ... based on how large the number is.
        private decimal GetBottomCashLimit()
            => Cash * (decimal)(NumberOfPossibleCombinations <= 1E5 ? 0.5 : (1 - 1E4 / NumberOfPossibleCombinations));

        /// <summary> Generates all stock quantity combinations and stores the best ones. </summary>
        public void GenerateCombinations()
        {
            Combinations.Clear(); // Clear list if running again.
            Parallel.For(0, (long)NumberOfPossibleCombinations, (id) =>
            {
                // It is much faster to get a combination cost than the combination itself.
                if (IsCostInRange(GetCombinationCostById((ulong)id)))
                    Combinations.Add(GetCombinationById((ulong)id));
            });
        }

        private bool IsCostInRange(decimal cost) => cost <= Cash && cost > BottomCashLimit; // Take only the best combinations.

        private decimal GetCombinationCostById(ulong id)
        {
            // Convert an ID into the combination as if all combinations were listed in order.
            // This algorithm basically converts base ten to base whatever the max stock quantities are.
            decimal cost = 0;
            for (int i = 0; i < MaxStockQuantities.Length; i++)
            {
                StockQuantity maxStockQuantity = MaxStockQuantities[i];
                cost += maxStockQuantity.Stock.Price * (uint)(id % (maxStockQuantity.Quantity + 1));
                id /= maxStockQuantity.Quantity + 1;
            }
            return cost;
        }

        private StockQuantity[] GetCombinationById(ulong id)
        {
            // Convert an ID into the combination as if all combinations were listed in order.
            // This algorithm basically converts base ten to base whatever the max stock quantities are.
            StockQuantity[] stockQuantities = new StockQuantity[MaxStockQuantities.Length];
            for (int i = 0; i < MaxStockQuantities.Length; i++)
            {
                StockQuantity maxStockQuantity = MaxStockQuantities[i];
                stockQuantities[i] = new StockQuantity
                {
                    Stock = maxStockQuantity.Stock,
                    Quantity = (uint)(id % (maxStockQuantity.Quantity + 1))
                };
                id /= maxStockQuantity.Quantity + 1;
            }
            return stockQuantities;
        }

        /// <summary> Updates the prices of all the stocks using the online public API. </summary>
        public void UpdateStockPrices()
        {
            StockPricesGetter.UpdatePrices();
            foreach (Stock stock in Stocks)
                stock.Price = StockPricesGetter[stock.Symbol];
        }

        /// <summary> Gets the best stock quantity combinations. </summary>
        /// <param name="top">Specifies how many combinations to return.</param>
        public IEnumerable<StockQuantity[]> GetBestCombinations(int top)
            => Combinations.Where(c => c != null).OrderByDescending(c => GetCombinationCost(c)).Take(top).AsEnumerable();

        /// <summary> Gets the cost of all the stocks and quantities in the given combination. </summary>
        /// <param name="combination">The given combination. </param>
        public static decimal GetCombinationCost(IEnumerable<StockQuantity> combination) => combination.Sum(s => s.Cost);
    }
}