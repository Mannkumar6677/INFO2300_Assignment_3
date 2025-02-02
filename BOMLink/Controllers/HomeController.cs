using BOMLink.Data;
using Microsoft.AspNetCore.Mvc;

namespace BOMLink.Controllers {
    public class HomeController : Controller {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly BOMLinkContext _context;

        public HomeController(BOMLinkContext context, IWebHostEnvironment webHostEnvironment) {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <returns>True if session is active, false if it is not</returns>
        public bool IsSessionActive() {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("Username"));
        }

        public IActionResult Index() {
            // Check if user is already logged in
            if (!IsSessionActive()) {
                TempData["SessionExpired"] = "Your session is expired. Please, log in";
                return RedirectToAction("Login", "User");
            }

            var username = HttpContext.Session.GetString("Username");
            @ViewBag.UserRole = _context.Users.FirstOrDefault(u => u.Username == username).Role;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            return View(user);
        }
    }
}