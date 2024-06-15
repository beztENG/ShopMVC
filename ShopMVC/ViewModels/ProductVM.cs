namespace ShopMVC.ViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!; 
        public string? ImageFileName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string CategoryName { get; set; } = null!; 
    }

    public class ProductDetailViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ImageFileName { get; set; }
        public decimal UnitPrice { get; set; }
        public string ShortDescription { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string FullDescription { get; set; } = null!; 
    }
}
