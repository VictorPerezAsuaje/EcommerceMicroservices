using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers;

public class TagDTO
{
    public string Name { get; set; }
}

public class CategoryDTO
{
    public string Name { get; set; }
}

public class ReviewDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    public double Rating { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

public class ProductDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public TagDTO Type { get; set; }
    public CategoryDTO Category { get; set; }
    public List<ReviewDTO> Reviews { get; set; } = new ();
    public double Rating => Reviews.Select(x => x.Rating).Sum() / Reviews.Count;
    public bool InStock { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }
}

public class ShopSearchFilter
{
    public string? TypeSelected { get; set; }
    public List<TagDTO> AvailableTypes { get; set; } = new ();
    public List<string> SelectedCategories { get; set; } = new ();
    public List<CategoryDTO> AvailableCategories { get; set; } = new ();

}

[Route("shop")]
public class ShopController : Controller
{
    #region Components 

    [Route("product-list")]
    public IActionResult ProductList(string? type = null, string? categories = null)
    {
        List<ProductDTO> products = new List<ProductDTO>(ProductData.Products);

        if(!string.IsNullOrWhiteSpace(type))
            products = products.Where(x => x.Type.Name == type).ToList();

        if (!string.IsNullOrWhiteSpace(categories))
        {
            string[] selectedCategories = categories.Split(',');
            products = products.Where(x => selectedCategories.Contains(x.Category.Name)).ToList();
        }

        return PartialView("Components/ProductList", products);
    }

    #endregion Components

    [Route("")]
    public IActionResult Index(string? type = null, string? category = null)
    {
        ShopSearchFilter filter = new ShopSearchFilter()
        {
            TypeSelected = type,
            AvailableTypes = ProductData.ProductTypes,
            AvailableCategories = ProductData.ProductCategories
        };

        filter.SelectedCategories.Add(category);

        return View(filter);
    }
}

public static class ProductData
{
    public static List<TagDTO> ProductTypes { get; } = new List<TagDTO>
    {
        new TagDTO { Name = "plants" },
        new TagDTO { Name = "bulbs" },
        new TagDTO { Name = "seeds" },
        new TagDTO { Name = "materials" },
        new TagDTO { Name = "tools" }
    };

    public static List<CategoryDTO> ProductCategories { get; } = new List<CategoryDTO>
    {
        new CategoryDTO { Name = "Perennials" },
        new CategoryDTO { Name = "Roses" },
        new CategoryDTO { Name = "Climbers" },
        new CategoryDTO { Name = "Indoor" },
        new CategoryDTO { Name = "Outdoor" },
        new CategoryDTO { Name = "Bedding" },
        new CategoryDTO { Name = "Tulips" },
        new CategoryDTO { Name = "Daffodils" },
        new CategoryDTO { Name = "Alliums" },
        new CategoryDTO { Name = "Iris" },
        new CategoryDTO { Name = "Amaryllis" },
        new CategoryDTO { Name = "Muscari" }
    };

    public static List<ProductDTO> Products { get; } = new List<ProductDTO>
    {
        new ProductDTO
        {
            Id = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
            Name = "Beautiful Perennial Plant",
            Price = 29.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[0],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "Gardener123", Email = "gardener@example.com", Rating = 5, Title = "Lovely Plant", Body = "This perennial is a gem in my garden!" },
                new ReviewDTO { Username = "FlowerLover", Email = "flower@example.com", Rating = 4, Title = "Vibrant Colors", Body = "The colors are amazing, and it blooms every year." }
            },
            InStock = true,
            Description = "A beautiful perennial plant that adds charm to any garden. Comes back year after year with vibrant blooms.",
            Details = "Height: 24 inches | Sunlight: Full sun to partial shade | Watering: Regular watering"
        },
        new ProductDTO
        {
            Id = "1e15b490-6e4a-4d98-8a93-698d40c491c5",
            Name = "Classic Rose Collection",
            Price = 39.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[1],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "RoseEnthusiast", Email = "rose@example.com", Rating = 5, Title = "Timeless Beauty", Body = "These roses are the epitome of elegance. Highly recommended!" },
                new ReviewDTO { Username = "GardenDesigner", Email = "designer@example.com", Rating = 4, Title = "Versatile Blooms", Body = "Used these roses in various designs. They never disappoint." }
            },
            InStock = true,
            Description = "A curated collection of classic roses, perfect for adding timeless beauty to your garden or special occasions.",
            Details = "Varieties: Red Rose, Pink Rose, White Rose | Height: Varies | Sunlight: Full sun | Watering: Moderate"
        },
        new ProductDTO
        {
            Id = "6a9c22e4-b90a-4d3c-b7e6-d1c9e8a9e6fe",
            Name = "Climbing Vine Sampler",
            Price = 45.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[2],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "GardenExplorer", Email = "explorer@example.com", Rating = 4, Title = "Vertical Delight", Body = "These climbing vines added a new dimension to my garden. Love them!" },
                new ReviewDTO { Username = "NatureLover", Email = "nature@example.com", Rating = 4, Title = "Natural Backdrops", Body = "Creates stunning backdrops against walls and fences. Impressed!" }
            },
            InStock = true,
            Description = "A sampler of climbing vines that bring vertical interest to your garden, creating stunning natural backdrops.",
            Details = "Varieties: Ivy, Clematis, Wisteria | Height: Varies | Sunlight: Full sun to partial shade | Watering: Moderate"
        },
        new ProductDTO
        {
            Id = "7171e32e-b058-4a07-aad3-dfe2653fbdcf",
            Name = "Indoor Oasis Collection",
            Price = 34.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[3],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "IndoorGardener", Email = "indoor@example.com", Rating = 5, Title = "Freshness Indoors", Body = "This collection transformed my indoor spaces into a green oasis. Love it!" },
                new ReviewDTO { Username = "PlantParent", Email = "parent@example.com", Rating = 4, Title = "Low Maintenance", Body = "Perfect for those who want greenery without a lot of fuss. Highly recommend." }
            },
            InStock = true,
            Description = "An indoor plant collection that enhances your indoor spaces, bringing greenery and freshness to your home.",
            Details = "Varieties: Peace Lily, Snake Plant, Pothos | Height: Varies | Light: Low to bright indirect light | Watering: Moderate"
        },
        new ProductDTO
        {
            Id = "c9d10399-2677-4f0a-b2b8-aa6474c324c5",
            Name = "Outdoor Elegance Set",
            Price = 49.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[4],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "OutdoorEntusiast", Email = "outdoor@example.com", Rating = 5, Title = "Elegant Touch", Body = "Transformed my outdoor area into an elegant retreat. Absolutely stunning!" },
                new ReviewDTO { Username = "GardenPartyHost", Email = "partyhost@example.com", Rating = 4, Title = "Perfect for Events", Body = "Used these plants for outdoor events. Guests were impressed!" }
            },
            InStock = true,
            Description = "An outdoor plant set that elevates your outdoor areas, making your garden or patio more inviting and beautiful.",
            Details = "Varieties: Ornamental Grass, Boxwood, Hydrangea | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductDTO
        {
            Id = "501c7ac2-6f72-4922-b385-0eb040c30346",
            Name = "Colorful Bedding Plants",
            Price = 27.99,
            Type = ProductData.ProductTypes[0],
            Category = ProductData.ProductCategories[5],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "BeddingDesigner", Email = "designer@example.com", Rating = 4, Title = "Vibrant Beds", Body = "These bedding plants created the colorful garden beds I envisioned. So happy with the results!" },
                new ReviewDTO { Username = "OutdoorEnthusiast", Email = "outdoor@example.com", Rating = 4, Title = "Easy to Grow", Body = "Perfect for those who want a vibrant garden with minimal effort. Great buy!" }
            },
            InStock = true,
            Description = "A collection of bedding plants that allows you to create cozy and colorful garden beds, transforming your outdoor space.",
            Details = "Varieties: Petunias, Marigolds, Zinnias | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductDTO
        {
            Id = "754db075-c927-4d1a-9d67-94bc39b9c4ea",
            Name = "Tulip Spectacular Mix",
            Price = 31.99,
            Type = ProductData.ProductTypes[1],
            Category = ProductData.ProductCategories[6],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "TulipLover", Email = "tulip@example.com", Rating = 5, Title = "Spectacular Blooms", Body = "This mix of tulips created a stunning display in my garden. Absolutely breathtaking!" },
                new ReviewDTO { Username = "GardenDesigner", Email = "designer@example.com", Rating = 4, Title = "Vibrant Colors", Body = "Used these tulips in various designs. Clients loved them!" }
            },
            InStock = true,
            Description = "A mix of tulips that brightens your garden with vibrant colors, adding a burst of charm in the spring.",
            Details = "Varieties: Red Tulip, Yellow Tulip, Pink Tulip | Height: Varies | Sunlight: Full sun | Watering: Regular"
        },
        new ProductDTO
        {
            Id = "6a5eab40-25d3-4cb6-96f3-7c3ff665f4b3",
            Name = "Daffodil Delight Collection",
            Price = 36.99,
            Type = ProductData.ProductTypes[1],
            Category = ProductData.ProductCategories[7],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "DaffodilFanatic", Email = "fanatic@example.com", Rating = 5, Title = "Delightful Blooms", Body = "These daffodils are a delight in my garden. Can't wait for spring every year!" },
                new ReviewDTO { Username = "NatureLover", Email = "nature@example.com", Rating = 4, Title = "Low Maintenance", Body = "Daffodils are perfect for those who want beautiful blooms without much effort." }
            },
            InStock = true,
            Description = "A collection of daffodils that welcomes spring with delightful blooms, a classic and beloved flower for your garden.",
            Details = "Varieties: Large-Cupped Daffodil, Trumpet Daffodil, Small-Cupped Daffodil | Height: Varies | Sunlight: Full sun to partial shade | Watering: Regular"
        },
        new ProductDTO
        {
            Id = "b6d5b2f9-167a-4a6b-bb15-2fc6e396666c",
            Name = "Allium Symphony Set",
            Price = 42.99,
            Type = ProductData.ProductTypes[1],
            Category = ProductData.ProductCategories[8],
            Reviews = new List<ReviewDTO>
            {
                new ReviewDTO { Username = "AlliumAdmirer", Email = "admirer@example.com", Rating = 4, Title = "Symphony of Blooms", Body = "This set of alliums created a symphony of blooms in my garden. Stunning and unique!" },
                new ReviewDTO { Username = "GardenArtist", Email = "artist@example.com", Rating = 4, Title = "Artistic Appeal", Body = "Used these alliums in my garden art installations. Added an artistic touch!" }
            },
            InStock = true,
            Description = "A set of alliums that adds a touch of elegance to your garden with their unique spherical flower heads.",
            Details = "Varieties: Giant Allium, Drumstick Allium, Purple Sensation Allium | Height: Varies | Sunlight: Full sun | Watering: Regular"
        }
    };
}