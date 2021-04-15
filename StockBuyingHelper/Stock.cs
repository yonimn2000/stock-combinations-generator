using System;

namespace StockBuyingHelper
{
    public class Stock
    {
        private string symbol;
        private decimal price;

        public string Symbol { get => symbol; set => symbol = value.ToUpper(); }
        public decimal Price { get => price; set => price = Math.Round(value, 2); }

        public override string ToString() => Symbol + ": $" + Price;
    }
}