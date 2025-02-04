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
*/

using BOMLink.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BOMLinkContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("BOMLinkContext")));

builder.Services.AddAuthentication("Cookies") // Define default authentication scheme
    .AddCookie("Cookies", options => {
        options.LoginPath = "/User/Login";  // Redirect to login if unauthorized
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });
builder.Services.ConfigureApplicationCookie(options => {
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Logout after 30 minutes
    options.SlidingExpiration = true; // Reset timer if the user is active
    options.LoginPath = "/User/Login"; // Redirect to login page if expired
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