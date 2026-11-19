using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries.GetByIdProducts;

public sealed record GetByIdProductsQuery(Guid Id) : IQuery<Result<GetByIdProductResponse>>;