using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Application;
using Services.Catalog.Application.Categories;

namespace Services.Catalog.UI;

[Route("categories")]
public class CategoryController : ControllerBase
{
    ICategoryService _categoryService; 

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok((await _categoryService.GetAllAsync()).ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        try
        {
            var result = (await _categoryService.GetByNameAsync(name)).ToResponseDTO();

            if (result.IsFailure)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }
}
