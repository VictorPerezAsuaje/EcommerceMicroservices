using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;


namespace Services.Catalog.Tests.Utilities;

internal class DbInitializer
{
    public const string StringWithMoreThan2000Characters = """
                Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla sodales et magna at sodales. Quisque eleifend bibendum diam, luctus bibendum ante scelerisque vitae. Duis odio magna, finibus ut porta eu, pulvinar eget dolor. Duis vehicula tortor eget augue aliquet tempus. Sed bibendum euismod dolor vel iaculis. Pellentesque eu dignissim dui. Duis sit amet leo odio. Sed lobortis urna non mi iaculis porttitor. Donec bibendum metus sed dolor congue sollicitudin. Nam sit amet tellus urna.

        Nullam sed molestie purus, in venenatis urna. Morbi ultricies condimentum ornare. Proin eget est non libero interdum convallis. Praesent at ipsum sapien. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla blandit tortor sagittis est fringilla malesuada. Sed commodo elit a ex posuere, eget pretium nisl faucibus. Praesent lacinia commodo interdum. Curabitur faucibus nisl eget metus imperdiet, varius mattis nisi auctor.

        Curabitur molestie metus et velit placerat, convallis euismod mauris commodo. Etiam tempus cursus mauris in vestibulum. Integer interdum interdum vehicula. Proin nec congue dui. Proin pharetra vel lacus et varius. Nunc odio ligula, euismod id dignissim vel, placerat non nibh. Morbi ipsum eros, ultrices ut efficitur eu, pellentesque accumsan elit. Integer congue vel nisl non rutrum. Etiam sed bibendum neque. Suspendisse sit amet justo a turpis pulvinar fringilla. Fusce semper volutpat varius. Donec sit amet pellentesque ipsum. Ut semper rutrum arcu quis finibus. Aliquam suscipit dignissim bibendum. Sed mattis lorem sit amet metus molestie sagittis.

        In eget ligula id dui euismod tincidunt ut a massa. Vivamus aliquam cursus lacinia. Curabitur nulla ante, varius a mi eget, consectetur venenatis nisl. Praesent facilisis metus odio, eu laoreet mauris ultricies vitae. Nam in fermentum diam. Nulla eu ex vitae tellus sodales luctus ut eget mauris. In hac habitasse platea dictumst. Fusce molestie lectus quam, in vestibulum mi cursus eu. Nulla porta turpis vehicula velit sodales lacinia. Nullam eget vehicula nunc. Proin pharetra volutpat erat vitae maximus. Suspendisse nec volutpat odio, vitae placerat ex. Nulla nisl neque, ultricies a sagittis ut, egestas sit amet neque.

        In quis eros eget tortor finibus aliquet. Morbi euismod lorem at tincidunt venenatis. Pellentesque sed metus vel mi interdum aliquam. Pellentesque nec mattis erat, nec fermentum tortor. Cras eget pulvinar nunc, ut placerat tellus. Proin augue urna, porta in augue sed, laoreet hendrerit turpis. Praesent pellentesque, nibh quis volutpat dignissim, ex tortor laoreet mi, sit amet venenatis dui odio nec tortor. Fusce vestibulum arcu eget erat suscipit, vitae blandit leo consectetur.

        In dapibus molestie felis, ac congue odio vulputate eget. Nullam pretium nisl ut neque placerat ultrices. Aliquam viverra odio a nibh efficitur volutpat. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Pellentesque malesuada ornare ex quis ultricies. In tincidunt velit arcu, ornare malesuada orci convallis eget. Phasellus ut leo ut augue elementum euismod. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Phasellus sollicitudin sapien non ligula aliquam posuere. Fusce sem est, pulvinar fringilla massa vitae, mollis sagittis nunc. Duis gravida tellus at bibendum pharetra.

        Etiam nec nulla ultricies, ultrices sem et, dictum diam. Vestibulum sagittis augue fermentum quam mollis, eget maximus tortor euismod. Sed a leo ut quam accumsan scelerisque vitae in enim. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Morbi in maximus est. Curabitur libero felis, ornare id mi sit amet, blandit placerat ex. Mauris a sapien non nunc faucibus vulputate. Proin enim nisl, consectetur quis euismod a, consectetur ut eros. Maecenas dapibus enim ac ullamcorper rutrum. Aenean gravida rhoncus nisi, posuere malesuada leo placerat eget. Nullam tincidunt mattis ligula vitae dapibus. Morbi sollicitudin facilisis leo. Aliquam laoreet neque placerat, accumsan mi vel, tempus elit.

        Vivamus placerat diam laoreet sapien auctor, non pharetra diam condimentum. Curabitur at nisi ut dolor mattis fringilla nec consequat leo. Donec pulvinar tempus augue, eu vulputate dui consectetur nec. Sed quis enim quis urna rutrum venenatis ut vel nisi. Vivamus tempor ligula eu tellus varius fermentum. Nunc scelerisque, nunc in tristique feugiat, dui urna sollicitudin metus, nec placerat dui nulla at lorem. Phasellus orci sapien, tincidunt ac eleifend ac, ornare a velit. Duis consequat purus luctus mi pharetra, nec maximus ligula vehicula. Fusce dignissim elementum est porttitor vulputate. Nunc sit amet felis et sapien euismod placerat a sed velit. Sed at tempus mi. Vestibulum vehicula odio nec leo volutpat maximus.

        Ut purus erat, condimentum sit amet consequat id, convallis congue quam. Maecenas vulputate magna purus, ut accumsan ipsum tincidunt at. Vestibulum sem dolor, sollicitudin ut arcu at, eleifend euismod dolor. Donec et leo ut orci ultrices bibendum. Etiam blandit egestas vestibulum. In in est et odio iaculis laoreet nec et quam. Duis interdum sapien est, sed viverra orci condimentum id. Aenean mollis dolor sagittis, accumsan enim sit amet, gravida urna. Morbi lacinia commodo metus nec mattis. Integer dapibus erat eu turpis fermentum, at vulputate quam placerat. Etiam finibus pharetra magna ut venenatis.

        Praesent nec vestibulum ante. Sed a neque augue. Aliquam semper erat vitae leo vulputate, nec interdum ex finibus. Morbi egestas suscipit rutrum. Suspendisse velit arcu, sodales sit amet urna eget, laoreet fringilla ipsum. Phasellus dictum non nibh id fringilla. Aliquam porta vel est quis rutrum.

        Sed quis sem varius, maximus dui sed, pretium quam. Interdum et malesuada fames ac ante ipsum primis in faucibus. Nulla porta lorem ac ante consectetur, non lacinia magna molestie. Quisque luctus tellus interdum libero pretium vehicula. Ut ultrices felis risus, eget euismod est consectetur pulvinar. Nunc sagittis ante quam, posuere blandit tellus pharetra non. Donec vel eleifend nunc. Vivamus ac est ac erat commodo blandit eget nec lacus. Pellentesque erat mauris, molestie sit amet hendrerit vel, blandit at nibh. Donec ac lorem quis mi pulvinar auctor a egestas leo. Ut fringilla rhoncus mauris sed scelerisque. Suspendisse ac felis a augue laoreet dictum. Maecenas porttitor magna turpis, nec tempor augue convallis ut. Integer lobortis bibendum dolor, eget mollis mauris fringilla non. Mauris commodo finibus turpis sit amet interdum. Donec nec lorem arcu.

        Nunc dapibus tristique posuere. Fusce volutpat non dolor non porttitor. Mauris quis pharetra urna, vel imperdiet lectus. Sed mattis sodales pellentesque. Etiam non.
        """;

    public static List<Category> Categories = new()
    {
        new Category(Category.Default),
        new Category("Electronics"),
        new Category("Home and Kitchen"),
        new Category("Sports and Outdoors"),
        new Category("Furniture"),
        new Category("Home and Garden"),
        new Category("Home and Office")
    };

    public static List<Tag> Tags = new()
    {
        new Tag(Tag.Default),
        new Tag("Gaming"),
        new Tag("Mobile"),
        new Tag("Appliance"),
        new Tag("Running"),
        new Tag("Photography"),
        new Tag("Audio")
    };

    public static List<Product> Products = new()
    {
        new Product(Guid.NewGuid(), "Laptop", 1200)
            .WithDescription("Powerful laptop")
            .WithDetails("8GB RAM, 512GB SSD, Core i7"),
        new Product(Guid.NewGuid(),"Smartphone", 800)
            .WithDescription("High-end smartphone")
            .WithDetails("6.5-inch display, dual camera, 128GB storage"),
        new Product(Guid.NewGuid(),"Coffee Maker", 50)
            .WithDescription("Single-serve coffee maker")
            .WithDetails("Brews coffee in under a minute"),
        new Product(Guid.NewGuid(),"Running Shoes", 80)
            .WithDescription("Comfortable running shoes")
            .WithDetails("Mesh upper, cushioned sole"),
        new Product(Guid.NewGuid(),"Bookshelf", 150)
            .WithDescription("Sturdy wooden bookshelf")
            .WithDetails("5 shelves, easy to assemble"),
        new Product(Guid.NewGuid(),"Digital Camera", 500)
            .WithDescription("High-resolution digital camera")
            .WithDetails("20MP, 4K video recording"),
        new Product(Guid.NewGuid(),"Bluetooth Headphones", 80)
            .WithDescription("Wireless Bluetooth headphones")
            .WithDetails("Over-ear design, noise-canceling"),
        new Product(Guid.NewGuid(),"Garden Tools Set", 60)
            .WithDescription("Complete garden tools kit")
            .WithDetails("Shovel, rake, pruning shears, gloves"),
        new Product(Guid.NewGuid(),"Yoga Mat", 30)
            .WithDescription("Non-slip yoga mat")
            .WithDetails("Extra thick for comfort"),
        new Product(Guid.NewGuid(),"Desk Lamp", 25)
            .WithDescription("LED desk lamp with adjustable brightness")
            .WithDetails("Modern design, energy-efficient")
    };

    public static void SeedData(CatalogDbContext context)
    {
        if (!context.Categories.Any())
            context.Categories.AddRange(Categories);

        if (!context.Tags.Any())
            context.Tags.AddRange(Tags);

        context.SaveChanges();

        if (!context.Products.Any())
        {
            context.Products.AddRange(Products);
        }

        context.SaveChanges();
    }
}

