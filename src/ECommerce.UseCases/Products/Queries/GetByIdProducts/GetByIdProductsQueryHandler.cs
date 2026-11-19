using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Queries.GetByIdProducts;

public sealed class GetByIdProductsQueryHandler(IReadRepository<Product> repository)
    : IRequestHandler<GetByIdProductsQuery, Result<GetByIdProductResponse>>
{
    public async Task<Result<GetByIdProductResponse>> Handle(GetByIdProductsQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(
            new ProductByIdSpecification(request.Id), cancellationToken);

        if (product is null)
            return Result<GetByIdProductResponse>.Failure(ProductErrors.NotFound);

        return product;
    }
}
