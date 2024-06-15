using Microsoft.AspNetCore.Mvc;
using ShopMVC.Helpers;
using ShopMVC.ViewModels;
using System.Security.Claims;

namespace ShopMVC.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string cartKey;
            if (User.Identity.IsAuthenticated)
            {
                var claimsPrincipal = User as ClaimsPrincipal;
                cartKey = MySetting.CartKeyPrefix + claimsPrincipal?.FindFirstValue("CustomerId");
            }
            else
            {
                cartKey = HttpContext.Session.GetString("AnonymousCartKey");
            }

            var cart = Request.Cookies.Get<List<CartItem>>(cartKey) ?? new List<CartItem>();

            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = (double)cart.Sum(p => p.TotalPrice)
            });
        }
    }
}
