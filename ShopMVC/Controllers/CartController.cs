using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data2;
using ShopMVC.ViewModels;
using ShopMVC.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ShopMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopMvcContext _db;

        public CartController(ShopMvcContext context)
        {
            _db = context;
        }

        private List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY)
                                        ?? new List<CartItem>();

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.ProductID == id); // English property name

            if (item == null)
            {
                var product = _db.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Product with ID {id} not found.";
                    return RedirectToAction("Error", "Home"); // Redirect to an error page 
                }

                item = new CartItem
                {
                    ProductID = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = (decimal)(product.UnitPrice ?? 0),
                    ImageFileName = product.Image ?? string.Empty,
                    Quantity = quantity,
                    ShippingFee = (decimal)product.ShippingFee
                };
                cart.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(MySetting.CART_KEY, cart);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var cart = Cart;
            var item = cart.SingleOrDefault(p => p.ProductID == id); // English property name

            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, cart);
            }
            return RedirectToAction("Index");
        }

        [Authorize] // Require authentication to access the checkout
        public IActionResult Checkout()
        {
            var cart = Cart;

            if (cart.Count == 0)
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            // Calculate total price (including shipping)
            decimal subtotal = cart.Sum(item => item.TotalPrice);
            decimal shippingTotal = cart.Sum(item => item.ShippingFee);
            decimal total = subtotal + shippingTotal;

            // Pass totals to the view
            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingTotal = shippingTotal;
            ViewBag.Total = total;

            return View(cart);
        }

    }

}


