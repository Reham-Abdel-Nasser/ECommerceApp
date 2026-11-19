using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(
    ICurrentUserService currentUser,
    IReadRepository<Order> repository)
    : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderResponse>>>
{
    public async Task<Result<PagedResult<OrderResponse>>> Handle(
        GetMyOrdersQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<PagedResult<OrderResponse>>.Failure(OrderErrors.Unauthorized);

        var userId = currentUser.UserId.Value;

        var countSpec = new OrdersByUserIdSpecification(userId);
        var listSpec = new OrdersByUserIdSpecification(
            userId,
            request.PageNumber,
            request.PageSize);

        var totalCount = await repository.CountAsync(
            countSpec,
            cancellationToken);

        var orders = await repository.ListAsync(
            listSpec,
            cancellationToken);

        IReadOnlyList<OrderResponse> items = [.. orders.Select(order => order.ToResponse())];

        return Result<PagedResult<OrderResponse>>.Success(
            new PagedResult<OrderResponse>(items, totalCount));
    }
}