using System.Collections.Generic;
using System.Linq;

namespace StockBuyingHelper
{
    public class StockCombinationGenerator
    {
        public Dictionary<string, StockQuantity> MaxStockQuantities { get; private set; }
        private List<StockQuantity> CurrentCombination { get; set; }

        public StockCombinationGenerator(List<StockQuantity> maxStockQuantities)
        {
            MaxStockQuantities = maxStockQuantities.ToDictionary(sq => sq.Stock.Symbol);

            // Set the current combination to 0000...
            CurrentCombination = maxStockQuantities.Select(s => new StockQuantity
            {
                Stock = s.Stock,
                Quantity = 0
            }).ToList();
        }

        public bool HasNextCombination()
        {
            // Check if all stocks are at max quantity.
            foreach (StockQuantity currentStockQuantity in CurrentCombination)
                if (currentStockQuantity.Quantity < MaxStockQuantities[currentStockQuantity.Stock.Symbol].Quantity)
                    return true;
            return false;
        }

        public List<StockQuantity> GetNextCombination()
        {
            // Do the same as 000 -> 001 and 099 -> 100 but use base max quantity instead of base 10.
            for (int i = 0; i < CurrentCombination.Count; i++)
            {
                StockQuantity currentStockQuantity = CurrentCombination[i];
                if (currentStockQuantity.Quantity < MaxStockQuantities[currentStockQuantity.Stock.Symbol].Quantity)
                {
                    currentStockQuantity.Quantity++;
                    break;
                }
                else
                    currentStockQuantity.Quantity = 0;
            }

            // Make a semi-deep copy.
            return CurrentCombination.Select(c => new StockQuantity
            {
                Stock = c.Stock,
                Quantity = c.Quantity
            }).ToList();
        }
    }
}