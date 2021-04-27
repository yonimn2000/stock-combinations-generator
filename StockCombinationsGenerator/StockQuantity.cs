namespace YonatanMankovich.StockCombinationsGenerator
{
    /// <summary> Represents a <see cref="StockCombinationsGenerator.Stock"/> and a quantity. </summary>
    public class StockQuantity
    {
        /// <summary> Gets or sets a stock. </summary>
        public Stock Stock { get; set; }
        
        /// <summary> Gets or sets a stock quantity. </summary>
        public uint Quantity { get; set; }
        
        /// <summary> Gets the stock cost. </summary>
        public decimal Cost => Quantity * Stock.Price;

        public override string ToString() => Quantity + " x " + Stock.Symbol;
    }
}