using Microsoft.Extensions.Options;
using WebClient.Services;

namespace WebClient.Services.Catalog.Tags;

public interface ITagService
{
    Task<ResponseDTO<TagGetDTO>> GetByNameAsync(string name);
    Task<ResponseDTO<List<TagGetDTO>>> GetAllAsync();
    Task<ResponseDTO> DeleteAsync(string name);
    Task<ResponseDTO> CreateAsync(TagPostDTO dto);
}

public class TagService : ITagService
{
    IBaseService _sender;
    ServiceOptions _catalogOptions;

    public TagService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _catalogOptions = services.Value.Catalog;
    }

    public async Task<ResponseDTO> CreateAsync(TagPostDTO dto)
    {
        return await _sender.SendAsync<Guid>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = _catalogOptions.BaseUrl + "/tags",
            Data = dto
        });
    }

    public async Task<ResponseDTO> DeleteAsync(string name)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDTO<List<TagGetDTO>>> GetAllAsync()
    {        
        return await _sender.SendAsync<List<TagGetDTO>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = _catalogOptions.BaseUrl + "/tags"
        });        
    }

    public async Task<ResponseDTO<TagGetDTO>> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}