using System.Collections.Generic;
using System.Linq;

namespace StockBuyingHelper
{
    public class BestStockQuantityBuyGenerator
    {
        public List<List<StockQuantity>> Combinations { get; } = new List<List<StockQuantity>>();
        public decimal Cash { get; set; }
        private StockCombinationGenerator Generator { get; set; }

        public BestStockQuantityBuyGenerator(List<StockQuantity> maxStockQuantities, decimal cash)
        {
            Generator = new StockCombinationGenerator(maxStockQuantities);
            Cash = cash;
        }

        public void GenerateCombinations()
        {
            while (Generator.HasNextCombination())
            {
                List<StockQuantity> combination = Generator.GetNextCombination();
                decimal cost = GetCombinationCost(combination);

                // Take only the best combinations.
                if (cost <= Cash && cost > Cash * 0.9m)
                    Combinations.Add(combination);
            }
        }

        public List<List<StockQuantity>> GetBestCombinations(int top) 
            => Combinations.OrderByDescending(c => GetCombinationCost(c)).Take(top).ToList();

        public static decimal GetCombinationCost(List<StockQuantity> combination) => combination.Sum(s => s.Cost);
    }
}