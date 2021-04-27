using System;
using System.Collections.Generic;
using System.Linq;
using YonatanMankovich.StockCombinationsGenerator;

namespace YonatanMankovich.StockCombinationsGeneratorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Stock Combinations Generator by Yonatan";

            string[] symbols = ConsoleHelpers.Prompt("Enter the stock ticker symbols separated by spaces").Trim().Split(' ');
            decimal cash = Math.Abs(decimal.Parse(ConsoleHelpers.Prompt("Enter the amount of cash you would like to trade ($)")));

            ConsoleHelpers.Highlight("Loading current stock prices...");
            CombinationsGenerator generator = new CombinationsGenerator(symbols, cash);

            Console.WriteLine();
            ConsoleHelpers.Underline($"Loaded initial stock prices (as of {DateTime.Now})");
            Console.WriteLine(string.Join("\n", generator.Stocks));

            Console.WriteLine();
            ConsoleHelpers.Underline("Max trade quantity of each stock");
            Console.WriteLine(string.Join("\n", (object[])generator.MaxStockQuantities));

            Console.WriteLine();
            ConsoleHelpers.Underline("Number of possible combinations");
            Console.WriteLine(generator.NumberOfPossibleCombinations.ToString("N0"));

            Console.WriteLine();
            ConsoleHelpers.Highlight("Generating combinations...");
            generator.GenerateCombinations();

            Console.WriteLine();
            if (generator.HasCombinations)
            {
                int baseCursorTop = Console.CursorTop;
                while (true)
                {
                    generator.UpdateStockPrices();

                    ConsoleHelpers.Underline($"Updated stock prices (as of {DateTime.Now})");
                    Console.WriteLine(string.Join("\n", generator.Stocks));

                    Console.WriteLine();
                    ConsoleHelpers.Highlight("Press ENTER to refresh the stock prices.");

                    Console.WriteLine();
                    ConsoleHelpers.Underline("Trade one of these quantities");
                    IEnumerable<StockQuantity[]> combinations = generator.GetBestCombinations(100);
                    Console.WriteLine(string.Join("\t", combinations.First().Select(c => c.Stock.Symbol)) + "\tLeft\tCost");
                    foreach (StockQuantity[] stockQuantityList in combinations)
                    {
                        decimal combinationCost = CombinationsGenerator.GetCombinationCost(stockQuantityList);
                        Console.WriteLine(string.Join("\t", stockQuantityList.Select(c => c.Quantity))
                             + "\t$" + (cash - combinationCost).ToString("N") + "\t$" + combinationCost.ToString("N"));
                    }
                    Console.SetWindowPosition(0, baseCursorTop);
                    Console.ReadLine();
                    Console.CursorTop = baseCursorTop;
                }
            }
            else
            {
                ConsoleHelpers.WriteInvesre("Not enough cash for a good combination...");
                Console.ReadLine();
            }
        }
    }
}