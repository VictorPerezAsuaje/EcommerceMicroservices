using Microsoft.Extensions.Options;
using WebClient.Services;

namespace WebClient.Services.Catalog.Categories;

public interface ICategoryService
{
    Task<ResponseDTO<CategoryGetDTO>> GetByNameAsync(string name);
    Task<ResponseDTO<List<CategoryGetDTO>>> GetAllAsync();
    Task<ResponseDTO> DeleteAsync(string name);
    Task<ResponseDTO> CreateAsync(CategoryPostDTO dto);
}

public class CategoryService : ICategoryService
{
    IBaseService _sender;
    ServiceOptions _catalogOptions;

    public CategoryService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _catalogOptions = services.Value.Catalog;
    }

    public async Task<ResponseDTO> CreateAsync(CategoryPostDTO dto)
    {
        return await _sender.SendAsync<Guid>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = _catalogOptions.BaseUrl + "/categories",
            Data = dto
        }, false);
    }

    public async Task<ResponseDTO> DeleteAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDTO<List<CategoryGetDTO>>> GetAllAsync()
    {
        return await _sender.SendAsync<List<CategoryGetDTO>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = _catalogOptions.BaseUrl + "/categories"
        }, false);
    }

    public async Task<ResponseDTO<CategoryGetDTO>> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}
