using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Commons.Interfaces;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Dtos;
using ECommerce.UseCases.Orders.Specifications;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    ICurrentUserService currentUser,
    IRepository<Order> repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CancelOrderCommand, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
            return Result<OrderResponse>.Failure(OrderErrors.Unauthorized);

        var order = await repository.FirstOrDefaultAsync(
            new OrderByIdForUserSpecification(request.Id, currentUser.UserId.Value, tracking: true),
            cancellationToken);

        if (order is null)
            return Result<OrderResponse>.Failure(OrderErrors.NotFound);

        if (order.UserId != currentUser.UserId.Value)
            return Result<OrderResponse>.Failure(OrderErrors.Forbidden);

        var cancelResult = order.Cancel();
        if (cancelResult.IsFailure)
            return Result<OrderResponse>.Failure(cancelResult.Error!);

        repository.Update(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Success(order.ToResponse());
    }
}