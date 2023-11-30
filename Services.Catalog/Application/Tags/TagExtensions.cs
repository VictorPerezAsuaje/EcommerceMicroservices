using Services.Catalog.Application.Categories;
using Services.Catalog.Domain;

namespace Services.Catalog.Application.Tags;

public static class TagExtensions
{
    public static List<TagGetDTO> ToListGetDTO(this List<Tag> tags)
        => tags.Select(ToGetDTO).ToList();

    public static IQueryable<TagGetDTO> ToQueryableGetDTO(this IQueryable<Tag> query)
        => query.Select(x => new TagGetDTO(x.Name));
    public static TagGetDTO ToGetDTO(this Tag tag)
        => new TagGetDTO(tag.Name);
}