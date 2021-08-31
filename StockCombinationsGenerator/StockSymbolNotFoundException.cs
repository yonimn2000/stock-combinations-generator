using System;

namespace YonatanMankovich.StockCombinationsGenerator
{
    public class StockSymbolNotFoundException : Exception
    {
        public string Symbol { get; }

        public StockSymbolNotFoundException(string symbol) : base($"The stock symbol '{symbol}' was not found.")
        {
            Symbol = symbol;
        }
    }
}