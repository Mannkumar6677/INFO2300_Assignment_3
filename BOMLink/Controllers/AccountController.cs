//using BOMLink.Models;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;

//public class AccountController : Controller {
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly SignInManager<ApplicationUser> _signInManager;

//    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
//        _userManager = userManager;
//        _signInManager = signInManager;
//    }

//    [HttpPost]
//    public async Task<IActionResult> Login(string username, string password) {
//        var user = await _userManager.FindByNameAsync(username);

//        if (user != null) {
//            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

//            if (result.Succeeded) {
//                // ✅ Get Roles from Identity
//                var roles = await _userManager.GetRolesAsync(user);
//                string userRole = roles.FirstOrDefault() ?? "User"; // Default to "User" if no role assigned

//                // ✅ Create Claims
//                var claims = new List<Claim> {
//                    new Claim(ClaimTypes.Name, user.UserName),
//                    new Claim("FullName", $"{user.FirstName} {user.LastName}"),
//                    new Claim(ClaimTypes.Role, userRole)  // ✅ Assign User Role Claim
//                };

//                var identity = new ClaimsIdentity(claims, "Cookies");
//                var principal = new ClaimsPrincipal(identity);

//                await HttpContext.SignInAsync("Cookies", principal);
//                return RedirectToAction("Index", "Dashboard");
//            }
//        }

//        TempData["InvalidLogin"] = "Invalid username or password.";
//        return View();
//    }
//}
