/* BOMLink/Program.cs
* Capstone Project
* Revision History
* Aline Sathler Delfino, 2025.01.25: Created, business layer.
* Aline Sathler Delfino, 2025.01.26: Database.
* Aline Sathler Delfino, 2025.02.01: Layout, Login page, hash password.
* Aline Sathler Delfino, 2025.02.02: Dashboard, Navbar, Profile Bubble, Logout.
* Alina Sathler Delfino, 2025.02.02: Manufacturer, Supplier, Customer, Job.
*/

using BOMLink.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(Options => { Options.IdleTimeout = TimeSpan.FromMinutes(30); });
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
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();