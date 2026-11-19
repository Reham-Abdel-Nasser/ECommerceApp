using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Types.Dtos;

namespace ECommerce.UseCases.Types;

public interface ITypeQueryService
{
    Task<IReadOnlyList<GetAllTypesResponse>> GetAllTypesAsync(CancellationToken ct = default);
}
