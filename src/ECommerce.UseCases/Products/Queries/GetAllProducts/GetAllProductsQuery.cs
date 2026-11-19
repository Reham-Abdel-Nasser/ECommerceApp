using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery : IQuery<Result<IReadOnlyList<GetAllProductsResponse>>>;