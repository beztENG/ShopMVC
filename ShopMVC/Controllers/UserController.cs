using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopMVC.Data2;
using System.Linq;
using System.Threading.Tasks;

public class UserController : Controller
{
    private readonly ShopMvcContext _db;

    public UserController(ShopMvcContext context)
    {
        _db = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _db.Users
            .Include(u => u.Customer)
            .Where(u => u.Role != 1) // Exclude admin users
            .ToListAsync();
        return View(users);
    }

    // POST: User/Activate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(int id, bool active)
    {
        var user = await _db.Users.Include(u => u.Customer).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null)
        {
            return NotFound();
        }

        user.Active = active;
        user.Customer.Active = active;

        _db.Update(user);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
