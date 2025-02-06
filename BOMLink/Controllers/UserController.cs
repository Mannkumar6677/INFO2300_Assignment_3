using BOMLink.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BOMLink.Controllers {
    public class UserController : Controller {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null) {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, lockoutOnFailure: false);

                if (result.Succeeded) {
                    var roles = await _userManager.GetRolesAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "User") // Assign first role
                    };

                    var identity = new ClaimsIdentity(claims, "Cookies");
                    var principal = new ClaimsPrincipal(identity);

                    var authProperties = new AuthenticationProperties {
                        IsPersistent = false,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30),
                        AllowRefresh = true
                    };

                    await HttpContext.SignInAsync("Cookies", principal, authProperties);
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            TempData["InvalidLogin"] = "Invalid username or password.";
            return View();
        }

        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}


