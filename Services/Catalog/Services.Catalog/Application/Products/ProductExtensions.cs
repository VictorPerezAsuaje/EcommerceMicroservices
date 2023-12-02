using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Reviews;
using Services.Catalog.Application.Tags;
using Services.Catalog.Domain;

namespace Services.Catalog.Application.Products;

public static class ProductExtensions
{
    public static Product UpdateProduct(this Product product, ProductPutDTO dto)
        => product.UpdateBasicData(dto.Name, dto.Price)
                    .WithDescription(dto.Description)
                    .WithDetails(dto.Details)
                    .WithCategory(dto.Category);

    public static Product ToProduct(this ProductPostDTO dto)
        => new Product(dto.Id, dto.Name, dto.Price)
                    .WithDescription(dto.Description)
                    .WithDetails(dto.Details)
                    .WithCategory(dto.Category);


    public static List<ProductGetDTO> ToListGetDTO(this List<Product> products)
        => products.Select(ToGetDTO).ToList();

    public static IQueryable<ProductGetDTO> ToQueryableGetDTO(this IQueryable<Product> query)
        => query.Select(x => new ProductGetDTO
        {
            Id = x.Id,
            Name = x.Name,
            Price = x.Price,
            Rating = x.Rating,
            Description = x.Description,
            Details = x.Details,
            Tags = x.Tags.ToListGetDTO(),
            Category = x.Category.ToGetDTO(),
            Reviews = x.Reviews.ToListGetDTO()
        });

    public static ProductGetDTO ToGetDTO(this Product product)
        => new ProductGetDTO()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Rating = product.Rating,
            Category = product.Category?.ToGetDTO(),
            Tags = product.Tags.ToListGetDTO(),
            Description = product.Description,
            Details = product.Details,
        };
}