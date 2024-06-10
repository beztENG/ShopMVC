using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Register(RegisterViewModel model, IFormFile? Image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Users.Any(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "This email address is already in use. Please use a different email.");
                        return View(model);
                    }

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

                    // Create the associated User entity
                    var user = new User
                    {
                        CustomerId = customer.CustomerId,
                        Password = customer.Password, // You might store it differently for security reasons (hash it)
                        Email = customer.Email,
                        Role = customer.Role,
                        Active = customer.Active,
                    };

                    db.Add(user);
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
                // Find the User by email
                var user = db.Users.Include(u => u.Customer).SingleOrDefault(u => u.Email == model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("error", "Invalid email or password.");
                }
                else
                {
                    if (user.Active == false)
                    {
                        ModelState.AddModelError("error", "This account has been locked. Please contact Admin.");
                    }
                    else
                    {
                        string saltKey = user.Customer.RandomKey; // Get salt from the associated Customer
                        string hashedPassword = model.Password.ToSHA256Hash(saltKey);

                        if (user.Password != hashedPassword)
                        {
                            ModelState.AddModelError("error", "Invalid email or password.");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Role, user.Role.ToString()) ,
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Name, user.Customer.FullName),
                                new Claim("CustomerId", user.CustomerId.ToString()),
                                new Claim(ClaimTypes.Role, "Customer")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(claimsPrincipal);


                            // After successful authentication, check the user's role
                            if (user.Role == 1) // Assuming 1 is the role for management access
                            {
                                // Redirect to the management system
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                // Redirect to the product list (or the original return URL)
                                if (Url.IsLocalUrl(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
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
