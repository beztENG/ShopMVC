using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopMVC.Data2;
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
        [HttpPost]
        public IActionResult Register(RegisterViewModel model, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _mapper.Map<Customer>(model);
                    customer.Active = true;
                    customer.Role = 0;

                    string saltKey = Guid.NewGuid().ToString();
                    customer.CustomerId = Guid.NewGuid().ToString();
                    customer.Password = model.Password.ToSHA256Hash(saltKey);
                    customer.RandomKey = saltKey;

                    if (Image != null && Image.Length > 0)
                    {
                        customer.Image = MyUtil.UploadImage(Image, "Customer");
                    }

                    db.Add(customer);
                    db.SaveChanges();
                    return Redirect("/Customer/LogIn");
                }
                catch (Exception ex)
                {
                    // Log the detailed exception message including inner exceptions
                    var errorMessage = $"Error occurred during registration: {ex.Message}\n{ex.StackTrace}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\nInner Exception: {ex.InnerException.Message}\n{ex.InnerException.StackTrace}";
                    }
                    ModelState.AddModelError("", errorMessage); // Temporarily expose detailed errors for debugging
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
                var customer = db.Customers.SingleOrDefault(cus => cus.Email == model.Email);
                if (customer == null)
                {
                    ModelState.AddModelError("error", "This customer is not exist in the database.");
                }
                else
                {
                    if (!customer.Active)
                    {
                        ModelState.AddModelError("error", "This account has been locked. Please contact Admin.");
                    }
                    else
                    {
                        string saltKey = customer.RandomKey;

                        string hashedPassword = model.Password.ToSHA256Hash(saltKey); // or ToSHA512Hash

                        if (customer.Password != hashedPassword)
                        {
                            ModelState.AddModelError("error", "Wrong information.");
                        }
                        else
                        {
                            var claims = new List<Claim>


                            {
                                new Claim(ClaimTypes.Email, customer.Email),
                                new Claim(ClaimTypes.Name, customer.FullName),
                                new Claim("CustomerId", customer.CustomerId),
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
            var customer = db.Customers.SingleOrDefault(cus => cus.CustomerId == User.FindFirst("CustomerID").Value);

            if (customer == null)
            {
                return NotFound();
            }

            var customerProfileVM = _mapper.Map<CustomerProfileVM>(customer);

            return View(customerProfileVM);
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateProfileImage(IFormFile Image)
        {
            if (Image != null && Image.Length > 0)
            {
                var customerId = User.FindFirst("CustomerId").Value;
                var customer = db.Customers.SingleOrDefault(cus => cus.CustomerId == customerId);

                if (customer == null)
                {
                    return NotFound();
                }

                try
                {
                    var newImage = MyUtil.UploadImage(Image, "Customer");

                    if (!string.IsNullOrEmpty(customer.Image))
                    {
                        MyUtil.DeleteImage(customer.Image, "Customer");
                    }

                    customer.Image = newImage;
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
