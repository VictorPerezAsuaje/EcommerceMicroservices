using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Application;
using Services.Catalog.Application.Categories;
using Services.Catalog.Application.Products;
using Services.Catalog.UI.Extensions;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryPostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        try
        {
            ResponseDTO result = (await _categoryService.CreateAsync(dto)).ToResponseDTO();

            if (result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        try
        {
            ResponseDTO result = (await _categoryService.DeleteAsync(name)).ToResponseDTO();

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
