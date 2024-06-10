using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Data2;
using ShopMVC.Helpers;
using ShopMVC.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ShopMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopMvcContext _db;

        public ProductController(ShopMvcContext context)
        {
            _db = context;
        }

        public async Task<IActionResult> Index(int? categoryId, int? price, int pageIndex = 1, int pageSize = 6)
        {
            var products = _db.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            ViewBag.MinPrice = 1; // Setting minimum price to 1
            ViewBag.MaxPrice = await products.MaxAsync(p => p.UnitPrice) ?? 1000; // Setting maximum price to the maximum price of the products in the filtered list
            ViewBag.SelectedPrice = price ?? (int)ViewBag.MaxPrice; // Default to max price if no price filter is set
            ViewBag.CategoryId = categoryId;

            if (price.HasValue)
            {
                products = products.Where(p => p.UnitPrice <= price.Value);
            }

            var result = products.Select(p => new ProductViewModel
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = (decimal)(p.UnitPrice ?? 0),
                ImageFileName = p.Image ?? "",
                ShortDescription = p.UnitDescription ?? "",
                CategoryName = p.Category.CategoryName
            });

            var paginatedList = await PaginatedList<ProductViewModel>.CreateAsync(result, pageIndex, pageSize);
            return View(paginatedList);
        }





        public async Task<IActionResult> Search(string? query, int pageIndex = 1, int pageSize = 6)
        {
            var products = _db.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p => p.ProductName.Contains(query));
            }

            var result = products.Select(p => new ProductViewModel
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = (decimal)(p.UnitPrice ?? 0),
                ImageFileName = p.Image ?? "",
                ShortDescription = p.UnitDescription ?? "",
                CategoryName = p.Category.CategoryName
            });

            var paginatedList = await PaginatedList<ProductViewModel>.CreateAsync(result, pageIndex, pageSize);
            return View(paginatedList);
        }

        public IActionResult Detail(int id)
        {
            var product = _db.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                TempData["Message"] = $"Product with ID {id} not found.";
                return RedirectToAction("Error", "Home");
            }

            var result = new ProductDetailViewModel
            {
                ProductID = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = (decimal)(product.UnitPrice ?? 0),
                FullDescription = product.Description ?? string.Empty,
                ImageFileName = product.Image,
                ShortDescription = product.UnitDescription ?? string.Empty,
                CategoryName = product.Category.CategoryName
            };

            return View(result);
        }


    }
}
