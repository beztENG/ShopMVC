using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data2;
using ShopMVC.ViewModels;
using ShopMVC.Helpers;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ShopMVC.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopMvcContext _db;

        public CartController(ShopMvcContext context)
        {
            _db = context;
        }

        private List<CartItem> GetCart()
        {
            string cartKey;

            if (User.Identity.IsAuthenticated)
            {
                // If logged in, use CustomerId
                cartKey = MySetting.CartKeyPrefix + User.FindFirstValue("CustomerId");
            }
            else
            {
                // If not logged in, try to get an existing anonymous cart key from session, or create a new one
                cartKey = HttpContext.Session.GetString("AnonymousCartKey");
                if (string.IsNullOrEmpty(cartKey))
                {
                    cartKey = MySetting.CartKeyPrefix + Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("AnonymousCartKey", cartKey);
                }
            }

            return HttpContext.Session.Get<List<CartItem>>(cartKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            string? cartKey = User.Identity.IsAuthenticated
                ? MySetting.CartKeyPrefix + User.FindFirstValue("CustomerId")
                : HttpContext.Session.GetString("AnonymousCartKey");
            HttpContext.Session.Set(cartKey, cart);
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductID == id); // English property name

            if (item == null)
            {
                var product = _db.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Product with ID {id} not found.";
                    return RedirectToAction("Error", "Home");
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

            SaveCart(cart); // Use SaveCart() to save the updated cart

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductID == id);

            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductID == productId);

            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = GetCart();

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

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string Address, string Notes, string SelectedPaymentMethod)
        {
            var cart = GetCart();

            if (cart.Count == 0)
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            try
            {
                var customerId = User.FindFirstValue("CustomerId");
                var customer = await _db.Customers.FindAsync(customerId);

                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Customer not found.";
                    return RedirectToAction("Checkout");
                }

                var order = new Order
                {
                    CustomerId = customerId,
                    OrderDate = DateTime.Now,
                    ShippedDate = DateTime.Now.AddDays(7), // Calculate shipped date (7 days after today)
                    FullName = customer.FullName, // Auto-populate from customer
                    Address = Address,
                    PaymentMethod = SelectedPaymentMethod,
                    ShippingMethod = "Standard",  // You can customize shipping methods
                    ShippingFee = (double)cart.Sum(item => item.ShippingFee),
                    Notes = Notes,
                    Active = true // Set order as active
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                foreach (var cartItem in cart)
                {
                    var product = await _db.Products.FindAsync(cartItem.ProductID);

                    if (product == null) continue;

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = cartItem.ProductID,
                        UnitPrice = (double)cartItem.UnitPrice,
                        Quantity = cartItem.Quantity,
                        Discount = product.Discount // Auto-populate from product
                    };

                    _db.OrderDetails.Add(orderDetail);

                    // Update the QuantitySold for the product
                    product.QuantitySold += cartItem.Quantity;
                }

                await _db.SaveChangesAsync();
                HttpContext.Session.Remove(MySetting.CartKeyPrefix);

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                ModelState.AddModelError("error", "Error occurred while placing an order.");

                TempData["ErrorMessage"] = "An error occurred while placing your order. Please try again later.";
                return RedirectToAction("Checkout");
            }
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _db.Orders.Include(o => o.OrderDetails).SingleOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }
            return View(order);
        }
    }
}
