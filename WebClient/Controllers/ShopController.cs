using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebClient.Models;
using WebClient.Services;
using WebClient.Services.Auth;
using WebClient.Services.Cart;
using WebClient.Services.Catalog.Categories;
using WebClient.Services.Catalog.Products;
using WebClient.Services.Catalog.Reviews;
using WebClient.Services.Catalog.Tags;
using WebClient.Utilities;

namespace WebClient.Controllers;

public class ShopSearchFilter
{
    public string? CategorySelected { get; set; }
    public List<CategoryGetDTO> AvailableCategories { get; set; } = new ();
    public List<string> SelectedTags { get; set; } = new ();
    public List<TagGetDTO> AvailableTags { get; set; } = new ();

}

[Route("shop")]
public class ShopController : Controller
{
    IProductService _productService;
    ICategoryService _categoryService;
    ITagService _tagService;
    public ShopController(IProductService productService, ICategoryService categoryService, ITagService tagService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _tagService = tagService;
    }

    [HttpPost("SeedShopData")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SeedProductData()
    {
        List<Task> tasks = new List<Task>();

        foreach (var tag in ProductData.TagsToSeed)
        {
            tasks.Add(_tagService.CreateAsync(tag));
        }

        foreach (var category in ProductData.CategoriesToSeed)
        {
            tasks.Add(_categoryService.CreateAsync(category));
        }

        await Task.WhenAll(tasks);

        tasks = new();

        foreach (var product in ProductData.ProductsToSeed)
        {
            tasks.Add(_productService.CreateAsync(product));
        }

        await Task.WhenAll(tasks);

        return RedirectToAction(nameof(Index));
    }

    [Route("")]
    public async Task<IActionResult> Index(string? category = null, string? tag = null)
    {
        ShopSearchFilter filter = new ShopSearchFilter();

        try
        {
            filter.CategorySelected = category;
            filter.SelectedTags.Add(tag);

            var categoryResult = await _categoryService.GetAllAsync();

            if (categoryResult.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error loading categories";
                    x.Message = "There was an error trying to retrieve the categories.";
                    x.Icon = NotificationIcon.error;
                });

                return View(filter);
            }

            var tagResult = await _tagService.GetAllAsync();

            if (tagResult.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error loading tags";
                    x.Message = "There was an error trying to retrieve the tags.";
                    x.Icon = NotificationIcon.error;
                });

                return View(filter);
            }

            filter.AvailableCategories = categoryResult.Value;
            filter.AvailableTags = tagResult.Value;
        }
        catch (Exception ex)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Error loading shop data";
                x.Message = "There was an error trying to load the page.";
                x.Icon = NotificationIcon.error;
            });
        }

        return View(filter);
    }

    [Route("product-list")]
    public async Task<IActionResult> ProductList(string? category = null, string? tags = null)
    {
        try
        {
            ResponseDTO<List<ProductGetDTO>> result = await _productService.GetAllAsync();

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error loading products";
                    x.Message = "There was an error trying to retrieve the product list.";
                    x.Icon = NotificationIcon.error;
                });

                return PartialView("Components/ProductList", result.Value);
            }

            List<ProductGetDTO> products = result.Value;

            if (!string.IsNullOrWhiteSpace(category))
                products = products.Where(x => x.Category.Name == category).ToList();

            if (!string.IsNullOrWhiteSpace(tags))
            {
                string[] selectedTags = tags.Split(',');
                products = products.Where(x => x.Tags.Any(y => selectedTags.Contains(y.Name))).ToList();
            }

            return PartialView("Components/ProductList", products);
        }
        catch(Exception ex) 
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Error loading product";
                x.Message = "There was an error trying to load the product list.";
                x.Icon = NotificationIcon.error;
            });

            return PartialView("Components/ProductList", new List<ProductGetDTO>());
        }        
    }

    [Route("{id}")]
    public async Task<IActionResult> Product(string id)
    {
        try
        {
            bool isValidGuid = Guid.TryParse(id, out Guid guid);

            if (!isValidGuid || guid == default(Guid))
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error loading product";
                    x.Message = "The selected product could not be loaded.";
                    x.Icon = NotificationIcon.error;
                });

                return RedirectToAction("Index");
            }

            ResponseDTO<ProductGetDTO> result = await _productService.GetByIdAsync(guid);

            if (result.IsFailure)
            {
                this.InvokeNotification(x =>
                {
                    x.Title = "Error loading product";
                    x.Message = "The selected product could not be loaded.";
                    x.Icon = NotificationIcon.error;
                });

                return RedirectToAction("Index");
            }

            return View(result.Value);
        }
        catch (Exception ex)
        {
            this.InvokeNotification(x =>
            {
                x.Title = "Error loading product";
                x.Message = "There was an error trying to load the product.";
                x.Icon = NotificationIcon.error;
            });

            return RedirectToAction("Index");
        }       

    }    

    
}

public static class ProductData
{
    public static List<TagGetDTO> Tags { get; } = new List<TagGetDTO>
    {
         new("Perennials"),
        new("Roses"),
        new("Climbers"),
        new("Indoor"),
        new("Outdoor"),
        new("Bedding"),
        new("Tulips"),
        new("Daffodils"),
        new("Alliums"),
        new("Iris"),
        new("Amaryllis"),
        new("Muscari")        
    };

    public static List<CategoryGetDTO> Categories { get; } = new List<CategoryGetDTO>
    {
       new("plants"),
        new("bulbs"),
        new("seeds"),
        new("materials"),
        new("tools")
    };

    public static List<ProductGetDTO> Products { get; } = new List<ProductGetDTO>
    {
        new ProductGetDTO
        {
            Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Name = "Beautiful Perennial Plant",
            Price = 29.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[0] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "Gardener123", Email = "gardener@example.com", Rating = 5, Title = "Lovely Plant", Body = "This perennial is a gem in my garden!" },
                new ReviewGetDTO { Username = "FlowerLover", Email = "flower@example.com", Rating = 4, Title = "Vibrant Colors", Body = "The colors are amazing, and it blooms every year." }
            },            
            Description = "A beautiful perennial plant that adds charm to any garden. Comes back year after year with vibrant blooms.",
            Details = "Height: 24 inches | Sunlight: Full sun to partial shade | Watering: Regular watering"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("1e15b490-6e4a-4d98-8a93-698d40c491c5"),
            Name = "Classic Rose Collection",
            Price = 39.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[1] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "RoseEnthusiast", Email = "rose@example.com", Rating = 5, Title = "Timeless Beauty", Body = "These roses are the epitome of elegance. Highly recommended!" },
                new ReviewGetDTO { Username = "GardenDesigner", Email = "designer@example.com", Rating = 4, Title = "Versatile Blooms", Body = "Used these roses in various designs. They never disappoint." }
            },
            
            Description = "A curated collection of classic roses, perfect for adding timeless beauty to your garden or special occasions.",
            Details = "Varieties: Red Rose, Pink Rose, White Rose | Height: Varies | Sunlight: Full sun | Watering: Moderate"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("6a9c22e4-b90a-4d3c-b7e6-d1c9e8a9e6fe"),
            Name = "Climbing Vine Sampler",
            Price = 45.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[2] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "GardenExplorer", Email = "explorer@example.com", Rating = 4, Title = "Vertical Delight", Body = "These climbing vines added a new dimension to my garden. Love them!" },
                new ReviewGetDTO { Username = "NatureLover", Email = "nature@example.com", Rating = 4, Title = "Natural Backdrops", Body = "Creates stunning backdrops against walls and fences. Impressed!" }
            },
            
            Description = "A sampler of climbing vines that bring vertical interest to your garden, creating stunning natural backdrops.",
            Details = "Varieties: Ivy, Clematis, Wisteria | Height: Varies | Sunlight: Full sun to partial shade | Watering: Moderate"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("7171e32e-b058-4a07-aad3-dfe2653fbdcf"),
            Name = "Indoor Oasis Collection",
            Price = 34.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[3] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "IndoorGardener", Email = "indoor@example.com", Rating = 5, Title = "Freshness Indoors", Body = "This collection transformed my indoor spaces into a green oasis. Love it!" },
                new ReviewGetDTO { Username = "PlantParent", Email = "parent@example.com", Rating = 4, Title = "Low Maintenance", Body = "Perfect for those who want greenery without a lot of fuss. Highly recommend." }
            },
            
            Description = "An indoor plant collection that enhances your indoor spaces, bringing greenery and freshness to your home.",
            Details = "Varieties: Peace Lily, Snake Plant, Pothos | Height: Varies | Light: Low to bright indirect light | Watering: Moderate"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("c9d10399-2677-4f0a-b2b8-aa6474c324c5"),
            Name = "Outdoor Elegance Set",
            Price = 49.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[4] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "OutdoorEntusiast", Email = "outdoor@example.com", Rating = 5, Title = "Elegant Touch", Body = "Transformed my outdoor area into an elegant retreat. Absolutely stunning!" },
                new ReviewGetDTO { Username = "GardenPartyHost", Email = "partyhost@example.com", Rating = 4, Title = "Perfect for Events", Body = "Used these plants for outdoor events. Guests were impressed!" }
            },
            
            Description = "An outdoor plant set that elevates your outdoor areas, making your garden or patio more inviting and beautiful.",
            Details = "Varieties: Ornamental Grass, Boxwood, Hydrangea | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("501c7ac2-6f72-4922-b385-0eb040c30346"),
            Name = "Colorful Bedding Plants",
            Price = 27.99,
            Category = ProductData.Categories[0],
            Tags = new() { ProductData.Tags[5] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "BeddingDesigner", Email = "designer@example.com", Rating = 4, Title = "Vibrant Beds", Body = "These bedding plants created the colorful garden beds I envisioned. So happy with the results!" },
                new ReviewGetDTO { Username = "OutdoorEnthusiast", Email = "outdoor@example.com", Rating = 4, Title = "Easy to Grow", Body = "Perfect for those who want a vibrant garden with minimal effort. Great buy!" }
            },
            
            Description = "A collection of bedding plants that allows you to create cozy and colorful garden beds, transforming your outdoor space.",
            Details = "Varieties: Petunias, Marigolds, Zinnias | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("754db075-c927-4d1a-9d67-94bc39b9c4ea"),
            Name = "Tulip Spectacular Mix",
            Price = 31.99,
            Category = ProductData.Categories[1],
            Tags = new() { ProductData.Tags[6] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "TulipLover", Email = "tulip@example.com", Rating = 5, Title = "Spectacular Blooms", Body = "This mix of tulips created a stunning display in my garden. Absolutely breathtaking!" },
                new ReviewGetDTO { Username = "GardenDesigner", Email = "designer@example.com", Rating = 4, Title = "Vibrant Colors", Body = "Used these tulips in various designs. Clients loved them!" }
            },
            
            Description = "A mix of tulips that brightens your garden with vibrant colors, adding a burst of charm in the spring.",
            Details = "Varieties: Red Tulip, Yellow Tulip, Pink Tulip | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("6a5eab40-25d3-4cb6-96f3-7c3ff665f4b3"),
            Name = "Daffodil Delight Collection",
            Price = 36.99,
            Category = ProductData.Categories[1],
            Tags = new() { ProductData.Tags[7] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "DaffodilFanatic", Email = "fanatic@example.com", Rating = 5, Title = "Delightful Blooms", Body = "These daffodils are a delight in my garden. Can't wait for spring every year!" },
                new ReviewGetDTO { Username = "NatureLover", Email = "nature@example.com", Rating = 4, Title = "Low Maintenance", Body = "Daffodils are perfect for those who want beautiful blooms without much effort." }
            },
            
            Description = "A collection of daffodils that welcomes spring with delightful blooms, a classic and beloved flower for your garden.",
            Details = "Varieties: Large-Cupped Daffodil, Trumpet Daffodil, Small-Cupped Daffodil | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductGetDTO
        {
            Id = Guid.Parse("b6d5b2f9-167a-4a6b-bb15-2fc6e396666c"),
            Name = "Allium Symphony Set",
            Price = 42.99,
            Category = ProductData.Categories[1],
            Tags = new() { ProductData.Tags[8] },
            Reviews = new List<ReviewGetDTO>
            {
                new ReviewGetDTO { Username = "AlliumAdmirer", Email = "admirer@example.com", Rating = 4, Title = "Symphony of Blooms", Body = "This set of alliums created a symphony of blooms in my garden. Stunning and unique!" },
                new ReviewGetDTO { Username = "GardenArtist", Email = "artist@example.com", Rating = 4, Title = "Artistic Appeal", Body = "Used these alliums in my garden art installations. Added an artistic touch!" }
            },
            
            Description = "A set of alliums that adds a touch of elegance to your garden with their unique spherical flower heads.",
            Details = "Varieties: Giant Allium, Drumstick Allium, Purple Sensation Allium | Height: Varies | Sunlight: Full sun | Watering: Regular"
        }
    };


    public static List<TagPostDTO> TagsToSeed { get; } = new List<TagPostDTO>
    {
         new() { Name = "Perennials" },
         new() { Name = "Roses" },
         new() { Name = "Climbers" },
         new() { Name = "Indoor" },
         new() { Name = "Outdoor" },
         new() { Name = "Bedding" },
         new() { Name = "Tulips" },
         new() { Name = "Daffodils" },
         new() { Name = "Alliums" },
         new() { Name = "Iris" },
         new() { Name = "Amaryllis" },
         new() { Name = "Muscari" }
    };

    public static List<CategoryPostDTO> CategoriesToSeed { get; } = new List<CategoryPostDTO>
    {
        new() { Name = "plants" },
        new() { Name = "bulbs" },
        new() { Name = "seeds" },
        new() { Name = "materials" },
        new() { Name = "tools" }
    };
    public static List<ProductPostDTO> ProductsToSeed { get; } = new List<ProductPostDTO>
    {
        new ProductPostDTO
        {
            Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
            Name = "Beautiful Perennial Plant",
            Price = 29.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[0].Name },
            Description = "A beautiful perennial plant that adds charm to any garden. Comes back year after year with vibrant blooms.",
            Details = "Height: 24 inches | Sunlight: Full sun to partial shade | Watering: Regular watering"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("1e15b490-6e4a-4d98-8a93-698d40c491c5"),
            Name = "Classic Rose Collection",
            Price = 39.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[1].Name },
            Description = "A curated collection of classic roses, perfect for adding timeless beauty to your garden or special occasions.",
            Details = "Varieties: Red Rose, Pink Rose, White Rose | Height: Varies | Sunlight: Full sun | Watering: Moderate"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("6a9c22e4-b90a-4d3c-b7e6-d1c9e8a9e6fe"),
            Name = "Climbing Vine Sampler",
            Price = 45.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[2].Name },
            Description = "A sampler of climbing vines that bring vertical interest to your garden, creating stunning natural backdrops.",
            Details = "Varieties: Ivy, Clematis, Wisteria | Height: Varies | Sunlight: Full sun to partial shade | Watering: Moderate"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("7171e32e-b058-4a07-aad3-dfe2653fbdcf"),
            Name = "Indoor Oasis Collection",
            Price = 34.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[3].Name },
            Description = "An indoor plant collection that enhances your indoor spaces, bringing greenery and freshness to your home.",
            Details = "Varieties: Peace Lily, Snake Plant, Pothos | Height: Varies | Light: Low to bright indirect light | Watering: Moderate"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("c9d10399-2677-4f0a-b2b8-aa6474c324c5"),
            Name = "Outdoor Elegance Set",
            Price = 49.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[4].Name },
            Description = "An outdoor plant set that elevates your outdoor areas, making your garden or patio more inviting and beautiful.",
            Details = "Varieties: Ornamental Grass, Boxwood, Hydrangea | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("501c7ac2-6f72-4922-b385-0eb040c30346"),
            Name = "Colorful Bedding Plants",
            Price = 27.99,
            Category = ProductData.Categories[0].Name,
            Tags = new() { ProductData.Tags[5].Name },
            Description = "A collection of bedding plants that allows you to create cozy and colorful garden beds, transforming your outdoor space.",
            Details = "Varieties: Petunias, Marigolds, Zinnias | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("754db075-c927-4d1a-9d67-94bc39b9c4ea"),
            Name = "Tulip Spectacular Mix",
            Price = 31.99,
            Category = ProductData.Categories[1].Name,
            Tags = new() { ProductData.Tags[6].Name },
            Description = "A mix of tulips that brightens your garden with vibrant colors, adding a burst of charm in the spring.",
            Details = "Varieties: Red Tulip, Yellow Tulip, Pink Tulip | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("6a5eab40-25d3-4cb6-96f3-7c3ff665f4b3"),
            Name = "Daffodil Delight Collection",
            Price = 36.99,
            Category = ProductData.Categories[1].Name,
            Tags = new() { ProductData.Tags[7].Name },
            Description = "A collection of daffodils that welcomes spring with delightful blooms, a classic and beloved flower for your garden.",
            Details = "Varieties: Large-Cupped Daffodil, Trumpet Daffodil, Small-Cupped Daffodil | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductPostDTO
        {
            Id = Guid.Parse("b6d5b2f9-167a-4a6b-bb15-2fc6e396666c"),
            Name = "Allium Symphony Set",
            Price = 42.99,
            Category = ProductData.Categories[1].Name,
            Tags = new() { ProductData.Tags[8].Name },
            Description = "A set of alliums that adds a touch of elegance to your garden with their unique spherical flower heads.",
            Details = "Varieties: Giant Allium, Drumstick Allium, Purple Sensation Allium | Height: Varies | Sunlight: Full sun | Watering: Regular"
        }
    };
}