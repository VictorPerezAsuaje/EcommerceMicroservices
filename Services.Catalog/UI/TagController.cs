using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Application;
using Services.Catalog.Application.Tags;

namespace Services.Catalog.UI;

[Route("tags")]
public class TagController : ControllerBase
{
    ITagService _tagService; 

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok((await _tagService.GetAllAsync()).ToResponseDTO());
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
            var result = (await _tagService.GetByNameAsync(name)).ToResponseDTO();

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
