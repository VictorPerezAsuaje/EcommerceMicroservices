using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using RabbitMQ.Client;
using Shared.MessageBus;
using System.Security.Claims;
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
    })
    .AddGoogle(opts =>
    {
        opts.ClientId = builder.Configuration["Google:ClientId"];
        opts.ClientSecret = builder.Configuration["Google:ClientSecret"];
        opts.CallbackPath = "/oauth/google";
    });


// AuthService
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMqOptions"));
builder.Services.AddScoped<IAuthService, AuthService>();

// CatalogService
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// CartService
builder.Services.AddScoped<ICartService, CartService>();

// OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddServiceBus(x =>
{
    x.User = builder.Configuration.GetSection(key: "RabbitMqOptions:User").Value;
    x.Password = builder.Configuration.GetSection(key: "RabbitMqOptions:Password").Value;
});

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
