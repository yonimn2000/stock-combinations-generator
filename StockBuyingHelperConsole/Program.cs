using System;
using System.Collections.Generic;
using System.Linq;
using YonatanMankovich.StockBuyingHelper;

namespace YonatanMankovich.StockBuyingHelperConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Stock Buying Helper by Yonatan";

            string[] symbols;
            decimal cash;
            List<StockQuantity> stocks = new List<StockQuantity>();

            symbols = Prompt("Enter the stock ticker symbols separated by spaces").Trim().Split(' ');
            cash = Math.Abs(decimal.Parse(Prompt("Enter the amount of cash you would like to invest ($)")));

            Highlight("Loading current stock prices...");
            StockPricesGetter stockPricesGetter = new StockPricesGetter(symbols);
            stockPricesGetter.UpdatePrices();
            stocks.AddRange(symbols.Select(symbol => new StockQuantity
            {
                Stock = new Stock
                {
                    Symbol = symbol,
                    Price = stockPricesGetter[symbol]
                },
                Quantity = (uint)Math.Floor(cash / stockPricesGetter[symbol])
            }));

            Console.WriteLine();
            Underline($"Loaded initial stock prices (as of {DateTime.Now})");
            foreach (Stock stock in stocks.Select(s => s.Stock))
                Console.WriteLine(stock);

            Console.WriteLine();
            Underline("Max buy quantity of each stock");
            foreach (StockQuantity stockMaxBuyQuantity in stocks)
                Console.WriteLine(stockMaxBuyQuantity);

            StockCombinationsGenerator generator = new StockCombinationsGenerator(stocks, cash);

            Console.WriteLine();
            Underline("Number of possible combinations");
            Console.WriteLine(generator.NumberOfPossibleCombinations.ToString("N0"));

            Console.WriteLine();
            Highlight("Generating combinations...");
            generator.GenerateCombinations();

            Console.WriteLine();
            if (generator.HasCombinations)
            {
                int baseCursorTop = Console.CursorTop;
                while (true)
                {
                    stockPricesGetter.UpdatePrices();
                    foreach (StockQuantity stock in stocks)
                        stock.Stock.Price = stockPricesGetter[stock.Stock.Symbol];

                    Underline($"Updated stock prices (as of {DateTime.Now})");
                    foreach (Stock stock in stocks.Select(s => s.Stock))
                        Console.WriteLine(stock);

                    Console.WriteLine();
                    Highlight("Press ENTER to refresh the stock prices.");

                    Console.WriteLine();
                    Underline("Buy one of these quantities");
                    IEnumerable<StockQuantity[]> combinations = generator.GetBestCombinations(100);
                    Console.WriteLine(string.Join("\t", combinations.First().Select(c => c.Stock.Symbol)) + "\tLeft\tCost");
                    foreach (StockQuantity[] stockQuantityList in combinations)
                    {
                        decimal combinationCost = StockCombinationsGenerator.GetCombinationCost(stockQuantityList);
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
                WriteInvesre("Not enough cash for a good combination...");
                Console.ReadLine();
            }
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