using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data;
using ShopMVC.Helpers;
using ShopMVC.ViewModels;

namespace ShopMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ShopMvcContext db;
        private readonly IMapper _mapper;

        public CustomerController(ShopMvcContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var khachHang = _mapper.Map<KhachHang>(model);
                    khachHang.HieuLuc = true;
                    khachHang.VaiTro = 0;

                    // Generate a random salt key
                    string saltKey = Guid.NewGuid().ToString();

                    // Hash the password using SHA-256 or SHA-512
                    khachHang.MatKhau = model.MatKhau.ToSHA256Hash(saltKey); // or ToSHA512Hash

                    // Store the salt key in the database
                    khachHang.RandomKey = saltKey;

                    if (Hinh != null && Hinh.Length > 0)
                    {
                        khachHang.Hinh = MyUtil.UpLoadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();
                    return Redirect("/Customer/LogIn");
                }
                catch (Exception ex)
                {
                    var mess = $"{ex.Message} shh";
                    ModelState.AddModelError("", "An error occurred while processing your registration. Please try again.");
                }
            }
            return View(model);
        }
        #endregion

        #region Login in
        [HttpGet]
        public IActionResult LogIn(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("error", "This customer is not exist in the database.");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("error", "This account has been locked. Please contact Admin.");
                    }
                    else
                    {
                        // Retrieve the salt key from the database
                        string saltKey = khachHang.RandomKey;

                        // Hash the entered password using the salt key
                        string hashedPassword = model.Password.ToSHA256Hash(saltKey); // or ToSHA512Hash

                        if (khachHang.MatKhau != hashedPassword)
                        {
                            ModelState.AddModelError("error", "Wrong information.");
                        }
                        else
                        {
                            // Xác thực thành công, tạo Claims và thực hiện đăng nhập
                            var claims = new List<Claim>


                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim("CustomerID", khachHang.MaKh),
                                new Claim(ClaimTypes.Role, "Customer")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/Product");
                            }
                        }
                    }
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == User.FindFirst("CustomerID").Value);

            if (khachHang == null)
            {
                return NotFound();
            }

            var customerProfileVM = _mapper.Map<CustomerProfileVM>(khachHang);

            return View(customerProfileVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateProfileImage(IFormFile Hinh)
        {
            if (Hinh != null && Hinh.Length > 0)
            {
                var customerId = User.FindFirst("CustomerID").Value;
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == customerId);

                if (khachHang == null)
                {
                    return NotFound();
                }

                try
                {
                    var newImage = MyUtil.UpLoadHinh(Hinh, "KhachHang");

                    if (!string.IsNullOrEmpty(khachHang.Hinh))
                    {
                        MyUtil.DeleteHinh(khachHang.Hinh, "KhachHang");
                    }

                    khachHang.Hinh = newImage;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while updating the profile picture. Please try again.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Please select a valid image file.");
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
