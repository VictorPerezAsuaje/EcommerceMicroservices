using Services.Catalog.Domain;

namespace Services.Catalog.Application.Categories;

public static class CategoryExtensions
{
    public static List<CategoryGetDTO> ToListGetDTO(this List<Category> categories)
        => categories.Select(ToGetDTO).ToList();
    public static IQueryable<CategoryGetDTO> ToQueryableGetDTO(this IQueryable<Category> query)
        => query.Select(x => new CategoryGetDTO(x.Name));
    public static CategoryGetDTO ToGetDTO(this Category category)
        => new CategoryGetDTO(category.Name);
}