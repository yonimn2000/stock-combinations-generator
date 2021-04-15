using System;
using System.Collections.Generic;
using System.Linq;

namespace StockBuyingHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] symbols;
            decimal cash;
            List<StockQuantity> stockMaxBuyQuantities = new List<StockQuantity>();

            Console.WriteLine("Enter the stock ticker symbols separated by spaces:");
            symbols = Console.ReadLine().Trim().Split(' ');

            Console.WriteLine();
            Console.WriteLine("Enter the amount of cash you want to invest ($):");
            cash = decimal.Parse(Console.ReadLine());

            Console.WriteLine();

            Console.WriteLine("Loading current prices...");
            StockPrices stockPrices = new StockPrices(symbols);
            stockPrices.UpdatePrices();
            stockMaxBuyQuantities.AddRange(symbols.Select(symbol => new StockQuantity
            {
                Stock = new Stock
                {
                    Symbol = symbol,
                    Price = stockPrices[symbol]
                },
                Quantity = (int)Math.Floor(cash / stockPrices[symbol])
            }));
            stockMaxBuyQuantities = stockMaxBuyQuantities.OrderByDescending(s => s.Stock.Price).ToList();

            Console.WriteLine();
            Console.WriteLine("Loaded:");
            foreach (Stock stock in stockMaxBuyQuantities.Select(s => s.Stock))
                Console.WriteLine(stock);

            Console.WriteLine();
            Console.WriteLine("Max quantity of each stock:");
            foreach (StockQuantity stockMaxBuyQuantity in stockMaxBuyQuantities)
                Console.WriteLine(stockMaxBuyQuantity);

            Console.WriteLine();
            Console.WriteLine("Crunching numbers...");
            BestStockQuantityBuyGenerator generator = new BestStockQuantityBuyGenerator(stockMaxBuyQuantities, cash);
            generator.GenerateCombinations();

            Console.WriteLine();
            if (generator.Combinations.Count > 0)
            {
                Console.WriteLine("Buy one of these quantities:");
                Console.WriteLine(string.Join("\t", generator.Combinations[0].Select(c => c.Stock.Symbol)) + "\tChange\tCost");
                foreach (List<StockQuantity> stockQuantityList in generator.GetBestCombinations(100))
                {
                    decimal combinationCost = BestStockQuantityBuyGenerator.GetCombinationCost(stockQuantityList);
                    Console.WriteLine(string.Join("\t", stockQuantityList.Select(c => c.Quantity))
                         + "\t$" + (cash - combinationCost) + "\t$" + combinationCost);
                }
            }
            else
                Console.WriteLine("Not enough cash for a good combination...");

            Console.ReadLine();
        }
    }
}