using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data;
using ShopMVC.ViewModels;

namespace ShopMVC.ViewComponents
{
    public class MenuTypeViewComponent : ViewComponent
    {
        private readonly ShopMvcContext db;
        public MenuTypeViewComponent(ShopMvcContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuTypeVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.TenLoai);
            return View(data); //Default.cshtml
        }
    }
}
