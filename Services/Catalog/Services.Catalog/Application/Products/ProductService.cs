using Microsoft.EntityFrameworkCore;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Reviews;
using Services.Catalog.Application.Tags;
using Services.Catalog.Domain;
using Services.Catalog.Infrastructure;

namespace Services.Catalog.Application.Products;

public interface IProductService
{
    Task<Result<ProductGetDTO>> GetByIdAsync(Guid id);
    Task<Result<List<ProductGetDTO>>> GetAllAsync();
    Task<Result> UpdateAsync(Guid productId, ProductPutDTO dto);
    Task<Result> DeleteAsync(Guid productId);
    Task<Result<Guid>> CreateAsync(ProductPostDTO dto);
}

public class ProductService : IProductService
{
    private readonly CatalogDbContext _context;

    public ProductService(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> CreateAsync(ProductPostDTO dto)
    {
        bool idAlreadyExists = await _context.Products.Where(x => x.Id == dto.Id).AnyAsync();

        if (idAlreadyExists)
            return Result.Fail<Guid>("A product with that id already exists, so it could not be created.");

        if(dto.Category is not null)
        {
            bool categoryExists = await _context.Categories.Where(x => x.Name == dto.Category).AnyAsync();

            if (!categoryExists)
                return Result.Fail<Guid>("The category assigned to the product does not exist, so the product could not be created.");
        }
        
        Product product = dto.ToProduct();

        if (dto.Tags?.Count > 0)
        {
            List<Tag> tags = await _context.Tags.Where(x => dto.Tags.Contains(x.Name)).ToListAsync();
            product.WithTags(tags);
        }

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return Result.Ok(product.Id);
    }

    public async Task<Result> DeleteAsync(Guid productId)
    {
        Product? product = await _context.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

        if (product is null)
            return Result.Fail("The selected product to delete does not exist.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(Guid productId, ProductPutDTO dto)
    {
        Product? product = await _context.Products.Where(x => x.Id == productId).FirstOrDefaultAsync();

        if (product is null)
            return Result.Fail("The selected product to update does not exist.");

        Category? category = await _context.Categories
                                            .Where(x => x.Name == dto.Category)
                                            .FirstOrDefaultAsync();

        if (category is null)
            dto.Category = Category.Default;

        product.UpdateProduct(dto);

        if (dto.Tags.Count > 0)
        {
            List<Tag> tags = await _context.Tags.Where(x => dto.Tags.Contains(x.Name)).ToListAsync();
            product.WithTags(tags);
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<ProductGetDTO>> GetByIdAsync(Guid id)
    {
        ProductGetDTO? product = await _context.Products
                                            .Include(x => x.Tags)
                                            .Include(x => x.Category)
                                            .Include(x => x.Reviews)
                                            .ToQueryableGetDTO()
                                            .FirstOrDefaultAsync(x => x.Id == id);

        if (product is null)
            return Result.Fail<ProductGetDTO>("The product could not be found.");

        return Result.Ok(product);
    }

    public async Task<Result<List<ProductGetDTO>>> GetAllAsync()
    {
        return Result.Ok(
            await _context.Products
                .Include(x => x.Tags)
                .Include(x => x.Category)
                .Include(x => x.Reviews)
                .ToQueryableGetDTO()
                .ToListAsync()
        );
    }


}
