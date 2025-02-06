/* BOMLink/Program.cs
* Capstone Project
* Revision History
* Aline Sathler Delfino, 2025.01.25: Created, business layer.
* Aline Sathler Delfino, 2025.01.26: Database.
* Aline Sathler Delfino, 2025.02.01: Layout, Login page, hash password.
* Aline Sathler Delfino, 2025.02.02: Dashboard, Navbar, Profile Bubble, Logout.
* Aline Sathler Delfino, 2025.02.02: Manufacturer, Supplier, Customer, Job.
* Aline Sathler Delfino, 2025.02.03: Job: Validation to avoid duplicate number, import/export.
* Aline Sathler Delfino, 2025.02.03: Part, Supplier-Manufacturer Mapping.
* Aline Sathler Delfino, 2025.02.04: Automatic Logout.
* Aline Sathler Delfino, 2025.02.04: User application using AspNetCore.Identity.
*/

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BOMLink.Data;
using BOMLink.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BOMLinkContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BOMLinkContext")));

// Register Identity Services with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<BOMLinkContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Auto logout after 30 min
    options.SlidingExpiration = true; // Extend session if active
    options.LoginPath = "/User/Login"; // Redirect to login
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();