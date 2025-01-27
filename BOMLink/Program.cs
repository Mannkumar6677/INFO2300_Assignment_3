/* BOMLink/Program.cs
* Capstone Project
* Revision History
* Aline Sathler Delfino, 2025.01.26: Created, business layer, database.
*/

using BOMLink.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession(Options => { Options.IdleTimeout = TimeSpan.FromMinutes(30); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BOMLinkContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("BOMLinkContext")));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();