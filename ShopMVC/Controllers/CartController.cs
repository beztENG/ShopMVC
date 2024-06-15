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
                cartKey = MySetting.CartKeyPrefix + User.FindFirstValue("CustomerId");
            }
            else
            {
                cartKey = HttpContext.Session.GetString("AnonymousCartKey");
                if (string.IsNullOrEmpty(cartKey))
                {
                    cartKey = MySetting.CartKeyPrefix + Guid.NewGuid().ToString();
                    HttpContext.Session.SetString("AnonymousCartKey", cartKey);
                }
            }

            return Request.Cookies.Get<List<CartItem>>(cartKey) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            string cartKey = User.Identity.IsAuthenticated
                ? MySetting.CartKeyPrefix + User.FindFirstValue("CustomerId")
                : HttpContext.Session.GetString("AnonymousCartKey");

            Response.Cookies.Set(cartKey, cart, 1440); // Save for 24 hours
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var cart = GetCart();
            var item = cart.SingleOrDefault(p => p.ProductID == id);

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

            SaveCart(cart);

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

            decimal subtotal = cart.Sum(item => item.TotalPrice);
            decimal shippingTotal = cart.Sum(item => item.ShippingFee);
            decimal total = subtotal + shippingTotal;

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
                    ShippedDate = DateTime.Now.AddDays(7),
                    FullName = customer.FullName,
                    Address = Address,
                    PaymentMethod = SelectedPaymentMethod,
                    ShippingMethod = "Standard",
                    ShippingFee = (double)cart.Sum(item => item.ShippingFee),
                    Notes = Notes,
                    Active = true
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
                        Discount = product.Discount
                    };

                    _db.OrderDetails.Add(orderDetail);

                    product.QuantitySold += cartItem.Quantity;
                }

                await _db.SaveChangesAsync();
                HttpContext.Session.Remove(MySetting.CartKeyPrefix);

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
            }
            catch (Exception ex)
            {
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
