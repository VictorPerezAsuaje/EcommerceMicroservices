using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Application;
using Services.Catalog.Application.Products;
using Services.Catalog.UI.Extensions;

namespace Services.Catalog.UI;

[Route("products")]
public class ProductController : ControllerBase
{
    IProductService _productService; 

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok((await _productService.GetAllAsync()).ToResponseDTO());
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = (await _productService.GetByIdAsync(id)).ToResponseDTO();

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
    public async Task<IActionResult> Create([FromBody] ProductPostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));            

        try
        {
            ResponseDTO<Guid> result = (await _productService.CreateAsync(dto)).ToResponseDTO();

            if(result.IsFailure)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            // Log exception
            return StatusCode(500, "There was an error trying to complete the request");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            ResponseDTO result = (await _productService.DeleteAsync(id)).ToResponseDTO();

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
