using System;

namespace YonatanMankovich.StockCombinationsGeneratorConsole
{
    internal static class ConsoleHelpers
    {
        internal static string Prompt(string prompt)
        {
            WriteInvesre(prompt);
            Console.Write("> ");
            string response = Console.ReadLine();
            Console.WriteLine();
            return response;
        }

        internal static void WriteInvesre(string text) => WriteInColors(text, Console.ForegroundColor, Console.BackgroundColor);

        internal static void Highlight(string text) => WriteInColors(text, ConsoleColor.Yellow, ConsoleColor.Black);

        internal static void WriteInColors(string text, ConsoleColor background, ConsoleColor foreground)
        {
            ConsoleColor originalBackgroundColor = Console.BackgroundColor;
            ConsoleColor originalForegroundColor = Console.ForegroundColor;
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(text);
            Console.BackgroundColor = originalBackgroundColor;
            Console.ForegroundColor = originalForegroundColor;
        }

        internal static void Underline(string text)
        {
            Console.WriteLine(text);
            for (int i = 0; i < text.Length; i++)
                Console.Write("-");
            Console.WriteLine();
        }
    }
}