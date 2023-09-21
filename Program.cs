using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplicationIceCreamProject.Data;
using Microsoft.Extensions.DependencyInjection;
using WebApplicationIceCreamProject.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<IceCreamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdminContext") ?? throw new InvalidOperationException("Connection string 'AdminContext' not found.")));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
// Add session support
builder.Services.AddSession(); // Add this line
var app = builder.Build();

var settings = builder.Configuration.GetSection("PayPalSettings").Get<PayPalSettings>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//builder.Services.AddScoped<ShoppingCartActions>();
app.UseHttpsRedirection();
app.UseStaticFiles();
// Add session middleware
app.UseSession(); // Add this line

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
