using Microsoft.AspNetCore.Mvc;
using ShopMVC.Helpers;
using ShopMVC.ViewModels;
using System.Security.Claims; // Add this line

namespace ShopMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string cartKey;
            if (User.Identity.IsAuthenticated)
            {
                // Cast User to ClaimsPrincipal
                var claimsPrincipal = User as ClaimsPrincipal;
                cartKey = MySetting.CartKeyPrefix + claimsPrincipal?.FindFirstValue("CustomerId");
            }
            else
            {
                cartKey = HttpContext.Session.GetString("AnonymousCartKey");
            }

            // Handle the case where the cartKey might be null (user is not logged in and no anonymous cart yet)
            var cart = HttpContext.Session.Get<List<CartItem>>(cartKey) ?? new List<CartItem>();

            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = (double)cart.Sum(p => p.TotalPrice)
            });
        }
    }
}
