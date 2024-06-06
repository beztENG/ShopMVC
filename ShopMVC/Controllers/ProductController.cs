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
            var query = _db.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (price.HasValue)
            {
                query = query.Where(p => p.UnitPrice <= price.Value); // Filter products with prices less than or equal to the selected price
            }

            ViewBag.MinPrice = await query.MinAsync(p => p.UnitPrice) ?? 0; // Default to 0 if null
            ViewBag.MaxPrice = await query.MaxAsync(p => p.UnitPrice) ?? 1000; // Default to 1000 if null

            var result = query.Select(p => new ProductViewModel
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
                return RedirectToAction("Error", "Home");  // Replace with your Error handling action/controller
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
