using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data2;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ShopMVC.ViewModels;
using System.Security.Claims;

namespace ShopMVC.ViewComponents
{
    public class AdminMenuViewComponent : ViewComponent
    {
        private readonly ShopMvcContext _db;

        public AdminMenuViewComponent(ShopMvcContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _db.Users.Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.CustomerId == userIdClaim);

            // Use null-conditional operator to simplify and avoid exceptions
            var adminMenuViewModel = new AdminMenuViewModel
            {
                CustomerId = user?.CustomerId,
                CustomerName = user?.Customer?.FullName,
                UserRole = user?.Role ?? 0
            };

            return View(adminMenuViewModel);
        }

    }
}
