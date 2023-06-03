namespace YonatanMankovich.StockCombinationsGenerator.Console
{
    internal static class ConsoleHelpers
    {
        internal static string? Prompt(string prompt)
        {
            WriteInvesre(prompt);
            System.Console.Write("> ");
            string? response = System.Console.ReadLine();
            System.Console.WriteLine();
            return response;
        }

        internal static void WriteInvesre(string text) => WriteInColors(text, System.Console.ForegroundColor, System.Console.BackgroundColor);

        internal static void Highlight(string text) => WriteInColors(text, ConsoleColor.Yellow, ConsoleColor.Black);

        internal static void WriteInColors(string text, ConsoleColor background, ConsoleColor foreground)
        {
            ConsoleColor originalBackgroundColor = System.Console.BackgroundColor;
            ConsoleColor originalForegroundColor = System.Console.ForegroundColor;
            System.Console.BackgroundColor = background;
            System.Console.ForegroundColor = foreground;
            System.Console.WriteLine(text);
            System.Console.BackgroundColor = originalBackgroundColor;
            System.Console.ForegroundColor = originalForegroundColor;
        }

        internal static void Underline(string text)
        {
            System.Console.WriteLine(text);
            for (int i = 0; i < text.Length; i++)
                System.Console.Write("-");
            System.Console.WriteLine();
        }
    }
}