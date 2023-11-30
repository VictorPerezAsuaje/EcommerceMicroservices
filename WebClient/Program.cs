using WebClient;
using WebClient.Services;
using WebClient.Services.Catalog.Categories;
using WebClient.Services.Catalog.Products;
using WebClient.Services.Catalog.Tags;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<APIServices>(builder.Configuration.GetSection("Services"));

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IBaseService, BaseService>();

// CatalogService
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

var app = builder.Build();

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
