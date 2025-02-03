using BOMLink.Data;
using BOMLink.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BOMLink.Controllers {
    public class UserController : Controller {
        private readonly BOMLinkContext _context;

        public UserController(BOMLinkContext context) {
            _context = context;
        }

        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == username);

            if (user != null) {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);

                if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded) {
                    // If rehash is needed, update the hash in the database
                    if (result == PasswordVerificationResult.SuccessRehashNeeded) {
                        user.HashedPassword = passwordHasher.HashPassword(user, password);
                        _context.SaveChanges();
                    }

                    var claims = new List<Claim>
                    {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

                    var identity = new ClaimsIdentity(claims, "Cookies");
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync("Cookies", principal);
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            TempData["InvalidLogin"] = "Invalid username or password.";
            return View();
        }


        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
