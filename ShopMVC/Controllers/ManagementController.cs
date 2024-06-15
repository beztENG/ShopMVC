using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Data2;
using ShopMVC.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace ShopMVC.Controllers
{
    public class ManagementController : Controller
    {
        private readonly ShopMvcContext _db;

        public ManagementController(ShopMvcContext context)
        {
            _db = context;
        }

        // Index action for managing products
        public async Task<IActionResult> ProductIndex()
        {
            var products = await _db.Products.ToListAsync();
            return View("/Views/Management/Product/Index.cshtml", products);
        }


        // GET: ProductCreate
        public IActionResult ProductCreate()
        {
            var categories = _db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");
            return View("/Views/Management/Product/Create.cshtml");
        }

        // POST: ProductCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(Product product, IFormFile? Image)
        {
            var categories = _db.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "CategoryName");

            if (product != null)
            {
                if (Image != null)
                {
                    product.Image = MyUtil.UploadImage(Image, "Product");
                }

                _db.Add(product);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(ProductIndex));
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            return View("/Views/Management/Product/Create.cshtml", product);
        }

        // Edit actions
        public async Task<IActionResult> ProductEdit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("/Views/Management/Product/Edit.cshtml", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(int id, Product product, IFormFile Image)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            var existingProduct = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            if (product != null && id != null)
            {
                try
                {
                    if (Image != null)
                    {
                        if (!string.IsNullOrEmpty(product.Image))
                        {
                            MyUtil.DeleteImage(existingProduct.Image, "Product");
                        }
                        product.Image = MyUtil.UploadImage(Image, "Product");
                    }
                    else
                    {
                        product.Image = existingProduct.Image;
                    }

                    _db.Update(product);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.Products.Any(e => e.ProductId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ProductIndex));
            }
            return View("/Views/Management/Product/Edit.cshtml", product);
        }

        // Delete actions
        public async Task<IActionResult> ProductDelete(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View("/Views/Management/Product/Delete.cshtml", product);
        }

        // POST: ProductDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(int id, string confirm)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Toggle the Active status
            product.Active = !product.Active;

            _db.Products.Update(product);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(ProductIndex));
        }

    }
}
