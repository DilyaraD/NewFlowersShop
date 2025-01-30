using Microsoft.EntityFrameworkCore;
using NewFlowersShop.Models;
using System.Text;
//using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<NewFlowersShopContext>(options =>
options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=NewFlowersShop;MultipleActiveResultSets=True;TrustServerCertificate=True;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;Application Name=NewFlowersShop;"));

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

builder.Services.AddControllersWithViews();

// Добавьте сервис для сессий
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseSession();

app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
