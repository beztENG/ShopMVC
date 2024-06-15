using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ShopMVC.Data2;

namespace ShopMVC.Controllers
{
    public class ProfitRankingController : Controller
    {
        private readonly ShopMvcContext _context;

        public ProfitRankingController(ShopMvcContext context)
        {
            _context = context;
        }

        // Method to calculate and update the profit for each product
        private async Task UpdateProductProfits()
        {
            var productProfits = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(group => new
                {
                    ProductId = group.Key,
                    TotalProfit = group.Sum(od => od.UnitPrice * od.Quantity),
                    TotalQuantitySold = group.Sum(od => od.Quantity) // Calculate total quantity sold
                })
                .ToListAsync();

            foreach (var item in productProfits)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Profit = item.TotalProfit;
                    product.QuantitySold = item.TotalQuantitySold; // Update QuantitySold
                }
            }

            await _context.SaveChangesAsync();
        }

        // Method to display products ranked by profit
        public async Task<IActionResult> RankingProfit()
        {
            await UpdateProductProfits();

            var rankedProducts = await _context.Products
                .Include(p => p.Category) // Include the Category navigation property
                .OrderByDescending(p => p.Profit)
                .ToListAsync();

            return View(rankedProducts);
        }
    }
}
