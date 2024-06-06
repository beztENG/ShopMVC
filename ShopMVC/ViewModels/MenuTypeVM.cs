namespace ShopMVC.ViewModels
{
    public class MenuTypeVM // More descriptive name
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = null!;
        public int ProductCount { get; set; } // Renamed for clarity
    }
}
