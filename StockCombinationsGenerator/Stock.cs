using System;

namespace YonatanMankovich.StockCombinationsGenerator
{
    /// <summary> Represents a stock with a symbol and price. </summary>
    public class Stock
    {
        private string symbol;
        private decimal price;
        private uint? portfolioWeight;

        /// <summary> Gets and sets the stock symbol. Converts to upper case when setting. </summary>
        public string Symbol { get => symbol; set => symbol = value.ToUpper(); }

        /// <summary> Gets and sets the stock price. Rounds to 2 decimal digits when setting. </summary>
        public decimal Price { get => price; set => price = Math.Round(value, 2); }

        /// <summary> Gets and sets the stock portfolio weight in percent where X = X%. </summary>
        public uint? PortfolioWeight
        {
            get => portfolioWeight;
            set
            {
                if (value > 100)
                    throw new ArgumentOutOfRangeException(nameof(PortfolioWeight), "Value was out of range. Accepted range: 0-100");

                portfolioWeight = value;
            }
        }

        public override string ToString() => Symbol + ": $" + Price.ToString("N");
    }
}