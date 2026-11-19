using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Enums;

namespace ECommerce.UseCases.Products.Queries.GetPagedProducts;

// usecase : 1. Command/query  2. Handler  3. Specification
public sealed record GetPagedProductsQuery(
        int PageNumber = 1,
        int PageSize = 5,
        string? Search = null,
        Guid? BrandId = null,
        Guid? TypeId = null,
        ProductSortField? SortBy = ProductSortField.Name,
        bool SortDescending = false

    ) : IQuery<Result<PagedResult<GetAllProductsResponse>>>;

