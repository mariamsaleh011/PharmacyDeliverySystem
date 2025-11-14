using Microsoft.EntityFrameworkCore;
using PharmacyDeliverySystem.Business;
using PharmacyDeliverySystem.Business.Interfaces;
using PharmacyDeliverySystem.Business.Managers;
// Business layer & DataAccess
using PharmacyDeliverySystem.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DbContext (PharmacyDeliveryContext) in the dependency injection container.
// This makes it possible to use the database (PharmacyDelivery) through Entity Framework Core.
builder.Services.AddDbContext<PharmacyDeliveryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registration of the Managers (Business Layer)
builder.Services.AddScoped<ICustomerManager, CustomerManager>();
builder.Services.AddScoped<IOrderManager, OrderManager>();
builder.Services.AddScoped<IProductManager, ProductManager>();
builder.Services.AddScoped<IPrescriptionManager, PrescriptionManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
