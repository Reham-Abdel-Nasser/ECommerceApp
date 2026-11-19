using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Brands.Specifications;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Brands.Queries.GetAllBrands;

public sealed class GetAllBrandsQueryHandler(IReadRepository<ProductBrand> Repository)
    : IRequestHandler<GetAllBrandsQuery, Result<IReadOnlyList<GetAllBrandsResponse>>>
//IBrandQueryService brandQueryService) 
{
    public async Task<Result<IReadOnlyList<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request,
        CancellationToken ct)
    {
        var brands = await Repository.ListAsync(new BrandsListSpecification(), ct);
        return Result<IReadOnlyList<GetAllBrandsResponse>>.Success(brands);
    }
}