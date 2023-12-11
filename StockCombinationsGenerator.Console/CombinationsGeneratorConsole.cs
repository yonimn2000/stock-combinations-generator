using ConsoleTables;
using YonatanMankovich.SimpleConsoleMenus;

namespace YonatanMankovich.StockCombinationsGenerator.Console
{
    public class CombinationsGeneratorConsole
    {
        private CombinationsGenerator? Generator { get; set; }
        private string[]? Symbols { get; set; }
        private decimal Cash { get; set; }
        private SimpleActionConsoleMenu Menu { get; } = new SimpleActionConsoleMenu("Next action:");
        private string ApiKey { get; set; }

        public CombinationsGeneratorConsole()
        {
            Menu.AddOption("Refresh stock prices", UpdatePricesAndShowCombinations);
            Menu.AddOption("Change amount", ChangeCash);
            Menu.AddOption("Change stocks", ChangeSymbols);
            Menu.AddOption("Start over", GetInputsAndRegenerate);
            Menu.AddOption("Exit", () => Environment.Exit(0));
        }

        public void Run()
        {
            LoadApiKey();
            GetInputsAndRegenerate();

            while (true)
            {
                System.Console.WriteLine();
                Menu.ShowHideAndDoAction();
            }
        }

        private void LoadApiKey()
        {
            const string ApiKeyFile = "Finnhub.txt";

            if (File.Exists(ApiKeyFile))
            {
                ApiKey = File.ReadAllText(ApiKeyFile);
            }
            else
            {
                ApiKey = GetUserApiKey();
                File.WriteAllText(ApiKeyFile, ApiKey);
            }
        }

        private void ChangeSymbols() => ClearActionRegenerate(() => Symbols = GetUserSymbols());

        private void ChangeCash() => ClearActionRegenerate(() => Cash = GetUserCash());

        private void GetInputsAndRegenerate()
           => ClearActionRegenerate(() =>
           {
               Symbols = GetUserSymbols();
               Cash = GetUserCash();
           });

        private void ClearActionRegenerate(Action action)
        {
            System.Console.Clear();
            action();
            Regenerate();
        }

        private void Regenerate()
        {
            CreateValidGenerator();

            ConsoleHelpers.Highlight("Loading current stock prices...");

            System.Console.WriteLine();
            ConsoleHelpers.Underline($"Loaded initial stock prices (as of {DateTime.Now})");
            System.Console.WriteLine(string.Join("\n", Generator!.Stocks));

            System.Console.WriteLine();
            ConsoleHelpers.Underline("Max trade quantity of each stock");
            System.Console.WriteLine(string.Join("\n", (object[])Generator.MaxStockQuantities));

            System.Console.WriteLine();
            ConsoleHelpers.Underline("Number of possible combinations");
            System.Console.WriteLine(Generator.NumberOfPossibleCombinations.ToString("N0"));

            System.Console.WriteLine();
            ConsoleHelpers.Highlight("Generating combinations...");
            Generator.GenerateCombinations();

            System.Console.WriteLine();
            if (Generator.HasCombinations)
                UpdatePricesAndShowCombinations();
            else
                ConsoleHelpers.WriteInColors("Not enough cash for a good combination...", ConsoleColor.Red, ConsoleColor.Black);
        }

        private void CreateValidGenerator()
        {
            bool valid = false;
            do
            {
                try
                {
                    Generator = new CombinationsGenerator(ApiKey, Symbols, Cash);
                    valid = true;
                }
                catch (StockSymbolNotFoundException e)
                {
                    ConsoleHelpers.WriteInColors(e.Message, ConsoleColor.Red, ConsoleColor.Black);
                    System.Console.WriteLine();
                    Symbols = Symbols!.Where(s => !s.Equals(e.Symbol)).ToArray();

                    if (Symbols.Length == 0)
                        Symbols = GetUserSymbols();
                    else
                    {
                        string? userInput = ConsoleHelpers.Prompt("Enter the stock ticker symbols separated by spaces (other than "
                                            + string.Join(", ", Symbols) + "). Leave blank if done.")?.Trim().ToUpper();

                        if (!string.IsNullOrWhiteSpace(userInput))
                            Symbols = Symbols.Concat(userInput.Split(' ')).ToArray();
                    }
                }
            } while (!valid);
        }

        private void UpdatePricesAndShowCombinations()
        {
            Generator!.UpdateStockPrices();

            ConsoleHelpers.Underline($"Updated stock prices (as of {DateTime.Now})");
            System.Console.WriteLine(string.Join("\n", Generator.Stocks));

            System.Console.WriteLine();
            ConsoleHelpers.Underline("Trade one of these quantities");

            IEnumerable<StockQuantity[]> combinations = Generator.GetBestCombinations(100);
            StockQuantity[] firstCombination = combinations.First();
            List<string> columns = new List<string>();

            foreach (StockQuantity stockQuantity in firstCombination)
                columns.Add(stockQuantity.Stock.Symbol + " (Q)");

            foreach (StockQuantity stockQuantity in firstCombination)
                columns.Add(stockQuantity.Stock.Symbol + " ($)");

            foreach (StockQuantity stockQuantity in firstCombination)
                columns.Add(stockQuantity.Stock.Symbol + " (%)");

            columns.Add("Left");
            columns.Add("Cost");

            ConsoleTable consoleTable = new ConsoleTable().AddColumn(columns);

            foreach (StockQuantity[] stockQuantityList in combinations)
            {
                List<string> rows = new List<string>();

                foreach (StockQuantity stockQuantity in stockQuantityList)
                    rows.Add(stockQuantity.Quantity.ToString("N0"));

                foreach (StockQuantity stockQuantity in stockQuantityList)
                    rows.Add((stockQuantity.Quantity * stockQuantity.Stock.Price).ToString("C"));

                foreach (StockQuantity stockQuantity in stockQuantityList)
                    rows.Add((stockQuantity.Quantity * stockQuantity.Stock.Price / Cash).ToString("P0"));

                decimal combinationCost = CombinationsGenerator.GetCombinationCost(stockQuantityList);

                rows.Add((Generator.Cash - combinationCost).ToString("C"));
                rows.Add(combinationCost.ToString("C"));
                consoleTable.AddRow(rows.ToArray());
            }

            consoleTable.Write(Format.Minimal);
        }

        private static decimal GetUserCash()
        {
            string? inputString;
            decimal cash;

            do inputString = ConsoleHelpers.Prompt("Enter the amount of cash you would like to trade ($)");
            while (!decimal.TryParse(inputString, out cash) || cash <= 0 || string.IsNullOrWhiteSpace(inputString));

            return Math.Abs(cash);
        }

        private static string[] GetUserSymbols()
        {
            string? inputString;

            do inputString = ConsoleHelpers.Prompt("Enter the stock ticker symbols separated by spaces");
            while (string.IsNullOrWhiteSpace(inputString));

            return inputString.Trim().ToUpper().Split(' ');
        }

        private static string GetUserApiKey()
        {
            string? inputString;

            do inputString = ConsoleHelpers.Prompt("Enter the Finnhub API key. You can get a free key by registering at https://finnhub.io");
            while (string.IsNullOrWhiteSpace(inputString));

            return inputString.Trim();
        }
    }
}