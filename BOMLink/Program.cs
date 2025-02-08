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
* Aline Sathler Delfino, 2025.02.05: User application using AspNetCore.Identity.
* Aline Sathler Delfino, 2025.02.06: User settings, Profile Picture, User Roles.
* Aline Sathler Delfino, 2025.02.07: User management, User registration, tooltips for buttons and uniform buttons.
* Aline Sathler Delfino, 2025.02.08: BOM: Model, Index, Create, Edit, Delete, Details, Search, Sort, Filter, Relationship and Clone.
*/

// test clone

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
    options.LoginPath = "/User/Login";  // Redirect to login if unauthorized
    options.LogoutPath = "/User/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true; // Reset timer if user is active
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options => {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.Configure<SecurityStampValidatorOptions>(options => {
    options.ValidationInterval = TimeSpan.Zero; // Force check on every request (Ensures the function to logout of all devices)
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