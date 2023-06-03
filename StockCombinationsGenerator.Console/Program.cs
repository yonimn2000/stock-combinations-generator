using YonatanMankovich.SimpleConsoleMenus;
using YonatanMankovich.StockCombinationsGenerator;
using YonatanMankovich.StockCombinationsGenerator.Console;

CombinationsGenerator? generator = null;
string[]? symbols = null;
decimal cash = 0;

Console.Title = "Stock Combinations Generator by Yonatan";

GetInputsAndRegenerate();

SimpleActionConsoleMenu menu = new SimpleActionConsoleMenu("Next action:");
menu.AddOption("Refresh stock prices", UpdatePricesAndShowCombinations);
menu.AddOption("Change amount", ChangeCash);
menu.AddOption("Change stocks", ChangeSymbols);
menu.AddOption("Start over", GetInputsAndRegenerate);
menu.AddOption("Exit", () => Environment.Exit(0));

while (true)
{
    Console.WriteLine();
    menu.ShowHideAndDoAction();
}

void ChangeSymbols() => ClearActionRegenerate(() => symbols = GetUserSymbols());

void ChangeCash() => ClearActionRegenerate(() => cash = GetUserCash());

void GetInputsAndRegenerate()
   => ClearActionRegenerate(() =>
   {
       symbols = GetUserSymbols();
       cash = GetUserCash();
   });

void ClearActionRegenerate(Action action)
{
    Console.Clear();
    action();
    Regenerate();
}

void Regenerate()
{
    CreateValidGenerator();

    ConsoleHelpers.Highlight("Loading current stock prices...");

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
        UpdatePricesAndShowCombinations();
    else
        ConsoleHelpers.WriteInColors("Not enough cash for a good combination...", ConsoleColor.Red, ConsoleColor.Black);
}

void CreateValidGenerator()
{
    bool valid = false;
    do
    {
        try
        {
            generator = new CombinationsGenerator(symbols, cash);
            valid = true;
        }
        catch (StockSymbolNotFoundException e)
        {
            ConsoleHelpers.WriteInColors(e.Message, ConsoleColor.Red, ConsoleColor.Black);
            Console.WriteLine();
            symbols = symbols.Where(s => !s.Equals(e.Symbol)).ToArray();

            if (symbols.Length == 0)
                symbols = GetUserSymbols();
            else
            {
                string? userInput = ConsoleHelpers.Prompt("Enter the stock ticker symbols separated by spaces (other than "
                                    + string.Join(", ", symbols) + "). Leave blank if done.")?.Trim().ToUpper();

                if (!string.IsNullOrWhiteSpace(userInput))
                    symbols = symbols.Concat(userInput.Split(' ')).ToArray();
            }
        }
    } while (!valid);
}

void UpdatePricesAndShowCombinations()
{
    generator.UpdateStockPrices();

    ConsoleHelpers.Underline($"Updated stock prices (as of {DateTime.Now})");
    Console.WriteLine(string.Join("\n", generator.Stocks));

    Console.WriteLine();
    ConsoleHelpers.Underline("Trade one of these quantities");
    IEnumerable<StockQuantity[]> combinations = generator.GetBestCombinations(100);
    Console.WriteLine(string.Join("\t", combinations.First().Select(c => c.Stock.Symbol)) + "\tLeft\tCost");
    foreach (StockQuantity[] stockQuantityList in combinations)
    {
        decimal combinationCost = CombinationsGenerator.GetCombinationCost(stockQuantityList);
        Console.WriteLine(string.Join("\t", stockQuantityList.Select(c => c.Quantity))
             + "\t$" + (generator.Cash - combinationCost).ToString("N") + "\t$" + combinationCost.ToString("N"));
    }
}

decimal GetUserCash()
{
    string? inputString;
    decimal cash;

    do inputString = ConsoleHelpers.Prompt("Enter the amount of cash you would like to trade ($)");
    while (!decimal.TryParse(inputString, out cash) || cash <= 0 || string.IsNullOrWhiteSpace(inputString));

    return Math.Abs(cash);
}

string[] GetUserSymbols()
{
    string? inputString;
    do inputString = ConsoleHelpers.Prompt("Enter the stock ticker symbols separated by spaces");
    while (string.IsNullOrWhiteSpace(inputString));
    return inputString.Trim().ToUpper().Split(' ');
}