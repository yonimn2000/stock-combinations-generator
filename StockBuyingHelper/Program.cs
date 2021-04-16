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

            symbols = Prompt("Enter the stock ticker symbols separated by spaces").Trim().Split(' ');
            cash = decimal.Parse(Prompt("Enter the amount of cash you want to invest ($)"));

            Highlight("Loading current prices...");
            StockPrices stockPrices = new StockPrices(symbols);
            stockPrices.UpdatePrices();
            stockMaxBuyQuantities.AddRange(symbols.Select(symbol => new StockQuantity
            {
                Stock = new Stock
                {
                    Symbol = symbol,
                    Price = stockPrices[symbol]
                },
                Quantity = (uint)Math.Floor(cash / stockPrices[symbol])
            }));
            stockMaxBuyQuantities = stockMaxBuyQuantities.OrderByDescending(s => s.Stock.Price).ToList();

            Console.WriteLine();
            Underline("Loaded");
            foreach (Stock stock in stockMaxBuyQuantities.Select(s => s.Stock))
                Console.WriteLine(stock);

            Console.WriteLine();
            Underline("Max quantity of each stock");
            foreach (StockQuantity stockMaxBuyQuantity in stockMaxBuyQuantities)
                Console.WriteLine(stockMaxBuyQuantity);

            BestStockQuantityBuyGenerator generator = new BestStockQuantityBuyGenerator(stockMaxBuyQuantities, cash);
           
            Console.WriteLine();
            Underline("Max combinations");
            Console.WriteLine(generator.GetMaxNumberOfCombinations().ToString("N0"));

            Console.WriteLine();
            Highlight("Crunching numbers...");
            generator.GenerateCombinations();

            Console.WriteLine();
            if (generator.Combinations.Count > 0)
            {
                do
                {
                    stockPrices.UpdatePrices();
                    foreach (StockQuantity stock in stockMaxBuyQuantities)
                        stock.Stock.Price = stockPrices[stock.Stock.Symbol];

                    Underline("Buy one of these quantities");
                    Console.WriteLine(string.Join("\t", generator.Combinations[0].Select(c => c.Stock.Symbol)) + "\tChange\tCost");
                    foreach (List<StockQuantity> stockQuantityList in generator.GetBestCombinations(10))
                    {
                        decimal combinationCost = BestStockQuantityBuyGenerator.GetCombinationCost(stockQuantityList);
                        Console.WriteLine(string.Join("\t", stockQuantityList.Select(c => c.Quantity))
                             + "\t$" + (cash - combinationCost) + "\t$" + combinationCost);
                    }
                    Console.ReadLine();
                } while (true);
            }
            else
                WriteInvesre("Not enough cash for a good combination...");

            Console.ReadLine();
        }

        public static string Prompt(string prompt)
        {
            WriteInvesre(prompt);
            Console.Write("> ");
            string response = Console.ReadLine();
            Console.WriteLine();
            return response;
        }

        public static void WriteInvesre(string text)
        {
            InvertConsoleColors();
            Console.WriteLine(text);
            InvertConsoleColors();
        }

        public static void Highlight(string text)
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
        }

        public static void Underline(string text)
        {
            Console.WriteLine(text);
            for (int i = 0; i < text.Length; i++)
                Console.Write("-");
            Console.WriteLine();
        }

        public static void InvertConsoleColors()
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = backgroundColor;
        }
    }
}