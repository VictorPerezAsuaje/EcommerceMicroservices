using Microsoft.EntityFrameworkCore;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;

namespace Services.Catalog.Application.Tags;

public interface ITagService
{
    Task<Result<TagGetDTO>> GetByNameAsync(string name);
    Task<Result<List<TagGetDTO>>> GetAllAsync();
    Task<Result> DeleteAsync(string name);
    Task<Result> CreateAsync(Tag tag);
}

public class TagService : ITagService
{
    private readonly CatalogDbContext _context;

    public TagService(CatalogDbContext context)
    {
        _context = context;
    }
    public async Task<Result> CreateAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(string name)
    {
        Tag? tag = await _context.Tags.Where(x => x.Name == name).FirstOrDefaultAsync();

        if (tag is null)
            return Result.Fail("The selected tag to delete does not exist.");

        _context.Tags.Remove(tag);
        return Result.Ok();
    }

    public async Task<Result<List<TagGetDTO>>> GetAllAsync()
        => Result.Ok(await _context.Tags.ToQueryableGetDTO().ToListAsync());

    public async Task<Result<TagGetDTO>> GetByNameAsync(string name)
    {
        TagGetDTO? tag = await _context.Tags
                                    .Where(x => x.Name == name)
                                    .ToQueryableGetDTO()
                                    .FirstOrDefaultAsync();

        if (tag is null)
            return Result.Fail<TagGetDTO>("The tag could not be found.");

        return Result.Ok(tag);
    }
}