namespace StockBuyingHelper
{
    public class StockQuantity
    {
        public Stock Stock { get; set; }
        public int Quantity { get; set; }
        public decimal Cost => Quantity * Stock.Price;

        public override string ToString()
        {
            return Quantity + " x " + Stock.Symbol;
        }
    }
}