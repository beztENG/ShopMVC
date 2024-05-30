using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data;
using ShopMVC.ViewModels;
using ShopMVC.Helpers;

namespace ShopMVC.Controllers
{
	public class CartController : Controller
	{
		private readonly ShopMvcContext db;

		public CartController(ShopMvcContext context)
		{
			db = context;
		}
		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>
			(MySetting.CART_KEY) ?? new List<CartItem>();
		public IActionResult Index()
		{
			return View(Cart);
		}

		public IActionResult AddToCart(int id, int quantity = 1)
		{
			var cart = Cart;
			var item = cart.SingleOrDefault(p => p.MaHh == id);
			if (item == null)
			{
				var hangHoa = db.HangHoas.SingleOrDefault(p => p.MaHh == id);
				if (hangHoa == null)
				{
					TempData["Message"] = $"Cannot find the product with id : {id}";
					return Redirect("404");
				}
				item = new CartItem
				{
					MaHh = hangHoa.MaHh,
					TenHh = hangHoa.TenHh,
					DonGia = hangHoa.DonGia ?? 0,
					Hinh = hangHoa.Hinh ?? string.Empty,
					SoLuong = quantity,
					PhiVanChuyen = hangHoa.PhiVanChuyen ?? 0,
				};
                cart.Add(item);
			}
			else
			{
				item.SoLuong += quantity;
			}
			HttpContext.Session.Set(MySetting.CART_KEY, cart);
			return RedirectToAction("Index");
		}

		public IActionResult RemoveCart(int id)
		{
			var cart = Cart;
			var item = cart.SingleOrDefault(p => p.MaHh == id);
			if (item != null)
			{
                cart.Remove(item);
				HttpContext.Session.Set(MySetting.CART_KEY, cart);
			}
			return RedirectToAction("Index");
		}
	}
}
