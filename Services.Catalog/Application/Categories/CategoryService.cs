using Microsoft.EntityFrameworkCore;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;

namespace Services.Catalog.Application.Categories;

public interface ICategoryService
{
    Task<Result<CategoryGetDTO>> GetByNameAsync(string name);
    Task<Result<List<CategoryGetDTO>>> GetAllAsync();
    Task<Result> DeleteAsync(string name);
    Task<Result> CreateAsync(CategoryPostDTO category);
}

public class CategoryService : ICategoryService
{
    private readonly CatalogDbContext _context;

    public CategoryService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result> CreateAsync(CategoryPostDTO category)
    {
        bool exists = await _context.Categories.AnyAsync(x => x.Name == category.Name);

        if (exists)
            return Result.Fail("The category already exists.");

        await _context.Categories.AddAsync(new Category(category.Name));
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(string name)
    {
        Category? category = await _context.Categories.Where(x => x.Name == name).FirstOrDefaultAsync();

        if (category is null)
            return Result.Fail("The selected category to delete does not exist.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<CategoryGetDTO>>> GetAllAsync()
        => Result.Ok(await _context.Categories.ToQueryableGetDTO().ToListAsync());

    public async Task<Result<CategoryGetDTO>> GetByNameAsync(string name)
    {
        CategoryGetDTO? category = await _context.Categories
                                    .Where(x => x.Name == name)
                                    .ToQueryableGetDTO()
                                    .FirstOrDefaultAsync();

        if (category is null)
            return Result.Fail<CategoryGetDTO>("The category could not be found.");

        return Result.Ok(category);
    }
}
