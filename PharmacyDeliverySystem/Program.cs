using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Business.Managers;
using PharmacyDeliverySystem.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<PharmacyDeliveryContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("PharmacyDeliverySystem.DataAccess")
    )
);

// Business layer
builder.Services.AddScoped<ICustomerManager, CustomerManager>();
builder.Services.AddScoped<IOrderManager, OrderManager>();
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IPrescriptionManager, PrescriptionManager>();
builder.Services.AddScoped<IReturnManager, ReturnManager>();
builder.Services.AddScoped<IRefundManager, RefundManager>();

// 🔐 Cookie Authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/CustomerAuth/Login";    // صفحة اللوجين
        options.LogoutPath = "/CustomerAuth/Logout";  // صفحة اللوج آوت
        options.AccessDeniedPath = "/Home/Index";     // لو مش مسموح له يدخل
        options.Cookie.Name = "PharmacyAuthCookie";
    });

// 👈 إضافة Authorization (مهم)
builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// ✅ لازم Authentication الأول وبعدين Authorization
app.UseAuthentication();
app.UseAuthorization();

// Static files عبر Aspire
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
