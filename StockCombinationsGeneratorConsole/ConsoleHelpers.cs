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

        internal static void WriteInvesre(string text)
        {
            InvertConsoleColors();
            Console.WriteLine(text);
            InvertConsoleColors();
        }

        internal static void InvertConsoleColors()
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = Console.ForegroundColor;
            Console.ForegroundColor = backgroundColor;
        }

        internal static void Highlight(string text)
        {
            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(text);
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;
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