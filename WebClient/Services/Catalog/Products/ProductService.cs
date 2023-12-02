using Microsoft.Extensions.Options;

namespace WebClient.Services.Catalog.Products;

public interface IProductService
{
    Task<ResponseDTO<ProductGetDTO>> GetByIdAsync(Guid id);
    Task<ResponseDTO<List<ProductGetDTO>>> GetAllAsync();
    Task<ResponseDTO> UpdateAsync(Guid productId, ProductPutDTO dto);
    Task<ResponseDTO> DeleteAsync(Guid productId);
    Task<ResponseDTO<Guid>> CreateAsync(ProductPostDTO dto);
}

public class ProductService : IProductService
{
    IBaseService _sender;
    ServiceOptions _catalogOptions;

    public ProductService(IBaseService sender, IOptions<APIServices> services)
    {
        _sender = sender;
        _catalogOptions = services.Value.Catalog;
    }

    public async Task<ResponseDTO<Guid>> CreateAsync(ProductPostDTO dto)
    {
        return await _sender.SendAsync<Guid>(new RequestDTO()
        {
            EndpointType = EndpointType.POST,
            Url = _catalogOptions.BaseUrl + "/products",
            Data = dto
        });
    }

    public async Task<ResponseDTO> DeleteAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public async Task<ResponseDTO<List<ProductGetDTO>>> GetAllAsync()
    {
        return await _sender.SendAsync<List<ProductGetDTO>>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = _catalogOptions.BaseUrl + "/products"
        });
    }

    public async Task<ResponseDTO<ProductGetDTO>> GetByIdAsync(Guid id)
    {
        return await _sender.SendAsync<ProductGetDTO>(new RequestDTO()
        {
            EndpointType = EndpointType.GET,
            Url = _catalogOptions.BaseUrl + "/products/" + id.ToString()
        });
    }

    public async Task<ResponseDTO> UpdateAsync(Guid productId, ProductPutDTO dto)
    {
        throw new NotImplementedException();
    }
}
