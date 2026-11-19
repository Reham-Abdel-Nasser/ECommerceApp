using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products;

public interface IProductQueryService
{
    Task<IReadOnlyList<GetAllProductsResponse>> GetAllProductsAsync(CancellationToken ct = default);
    Task<GetByIdProductResponse?> GetByIdProductAsync(Guid Id, CancellationToken ct = default);

}
