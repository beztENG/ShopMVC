using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data2;
using ShopMVC.ViewModels;
using System.Linq;

namespace ShopMVC.ViewComponents
{
    public class MenuTypeViewComponent : ViewComponent // Renamed for clarity
    {
        private readonly ShopMvcContext _db;

        public MenuTypeViewComponent(ShopMvcContext context)
        {
            _db = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _db.Categories  
                .Select(category => new MenuTypeVM
                {
                    CategoryID = category.CategoryId,
                    CategoryName = category.CategoryName,
                    ProductCount = category.Products.Count(p => p.Active) // Assuming Products is a navigation property
                })
                .OrderBy(p => p.CategoryName);

            return View(categories); // Use the default view "_CategoryMenu" or create a specific view for this component
        }
    }
}
