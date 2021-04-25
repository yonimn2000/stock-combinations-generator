using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockBuyingHelper
{
    public class BestStockQuantityBuyGenerator
    {
        public IList<StockQuantity[]> Combinations { get; } = new List<StockQuantity[]>();
        public ulong NumberOfPossibleCombinations { get; }

        private decimal Cash { get; }
        private decimal BottomCashLimit { get; }
        private StockQuantity[] MaxStockQuantities { get; }

        public BestStockQuantityBuyGenerator(IEnumerable<StockQuantity> maxStockQuantities, decimal cash)
        {
            MaxStockQuantities = maxStockQuantities.ToArray();
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

        public List<StockQuantity[]> GetBestCombinations(int top)
            => Combinations.Where(c => c != null).OrderByDescending(c => GetCombinationCost(c)).Take(top).ToList();

        public static decimal GetCombinationCost(IEnumerable<StockQuantity> combination) => combination.Sum(s => s.Cost);
    }
}