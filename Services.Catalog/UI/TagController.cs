using Microsoft.AspNetCore.Mvc;
using Services.Catalog.Application;
using Services.Catalog.Application.Tags;
using Services.Catalog.UI.Extensions;

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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TagPostDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDTO(false, ModelState.GetErrorsAsString()));

        try
        {
            ResponseDTO result = (await _tagService.CreateAsync(dto)).ToResponseDTO();

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
            ResponseDTO result = (await _tagService.DeleteAsync(name)).ToResponseDTO();

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
}
