using Microsoft.AspNetCore.Authentication.Cookies;
using WebClient;
using WebClient.Services;
using WebClient.Services.Auth;
using WebClient.Services.Cart;
using WebClient.Services.Catalog.Categories;
using WebClient.Services.Catalog.Products;
using WebClient.Services.Catalog.Tags;
using WebClient.Services.Order;
using WebClient.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<APIServices>(builder.Configuration.GetSection("Services"));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.ExpireTimeSpan = TimeSpan.FromDays(1);
        opts.LoginPath = "/login";
    });

// AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// CatalogService
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// CartService
builder.Services.AddScoped<ICartService, CartService>();

// CartService
builder.Services.AddScoped<IOrderService, OrderService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<SessionMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
