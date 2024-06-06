namespace ShopMVC.ViewModels
{
    public class CartItem
    {
        public int ProductID { get; set; }
        public string? ImageFileName { get; set; } // Made nullable as images might not always be available
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }  // Use decimal for currency accuracy
        public decimal ShippingFee { get; set; } // Use decimal for currency accuracy

        public int Quantity { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice + ShippingFee; // Calculate total price with decimals
    }
}
