using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Data;
using ShopMVC.Helpers;
using ShopMVC.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ShopMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopMvcContext db;
        public ProductController(ShopMvcContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Index(int? loai, int pageIndex = 1, int pageSize = 6)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
            }

            var result = hangHoas.Select(p => new ProductVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            var paginatedList = await PaginatedList<ProductVM>.CreateAsync(result, pageIndex, pageSize);
            return View(paginatedList);
        }

        public async Task<IActionResult> Search(string? query, int pageIndex = 1, int pageSize = 6)
        {
            var hangHoas = db.HangHoas.AsQueryable();

            if (query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            var result = hangHoas.Select(p => new ProductVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                DonGia = p.DonGia ?? 0,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTaDonVi ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            var paginatedList = await PaginatedList<ProductVM>.CreateAsync(result, pageIndex, pageSize);
            return View(paginatedList);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);

            if (data == null)
            {
                TempData["Message"] = $"Cannot found the product with id {id}";
                return Redirect("/404");
            }

            var result = new ProductDetailVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
            };

            return View(result);
        }
    }
}
