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


        public async Task<IActionResult> Index(int? categoryId, int? price, string? query, int pageIndex = 1, int pageSize = 6)
        {
            var products = _db.Products.Include(p => p.Category).Where(p => p.Active); // Include Category here

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value);
            }

            if (price.HasValue)
            {
                products = products.Where(p => p.UnitPrice <= price.Value);
            }

            // Search Filter
            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p => p.ProductName.Contains(query));
            }

            ViewBag.MinPrice = 1;
            ViewBag.MaxPrice = await _db.Products.Where(p => p.Active).MaxAsync(p => p.UnitPrice) ?? 1000;
            ViewBag.SelectedPrice = price ?? (int)ViewBag.MaxPrice;
            ViewBag.CategoryId = categoryId;

            var result = products.Select(p => new ProductViewModel // Map to ProductViewModel
            {
                ProductID = p.ProductId,
                ProductName = p.ProductName,
                UnitPrice = (decimal)(p.UnitPrice ?? 0),
                ImageFileName = p.Image ?? "",
                ShortDescription = p.UnitDescription ?? "",
                CategoryName = p.Category.CategoryName // Make sure Category is included
            });

            var paginatedList = await PaginatedList<ProductViewModel>.CreateAsync(result, pageIndex, pageSize);
            ViewBag.SearchQuery = query; // Store the search query in ViewBag to pass to _Pagination

            return View(paginatedList); // Return the same Index view
        }



        public IActionResult Detail(int id)
        {
            var product = _db.Products
                .Include(p => p.Category)
                .SingleOrDefault(p => p.ProductId == id && p.Active);

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
