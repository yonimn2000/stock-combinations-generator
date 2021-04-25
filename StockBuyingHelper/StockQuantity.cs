namespace YonatanMankovich.StockBuyingHelper
{
    public class StockQuantity
    {
        public Stock Stock { get; set; }
        public uint Quantity { get; set; }
        public decimal Cost => Quantity * Stock.Price;
        public override string ToString() => Quantity + " x " + Stock.Symbol;
    }
}