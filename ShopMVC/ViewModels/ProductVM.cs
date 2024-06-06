namespace ShopMVC.ViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!; // Assuming product names are required
        public string? ImageFileName { get; set; } // Made nullable (images might be optional)
        public decimal UnitPrice { get; set; } // Use decimal for accuracy with money
        public string ShortDescription { get; set; } = null!; // Assuming you'll always have a short description
        public string CategoryName { get; set; } = null!; // Assuming each product has a category
    }

    public class ProductDetailViewModel // More descriptive name
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageFileName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string FullDescription { get; set; } = null!; // Assuming you'll always have a full description
    }
}
