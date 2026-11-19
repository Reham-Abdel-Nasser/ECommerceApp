using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Dtos;

namespace ECommerce.UseCases.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid Id) : ICommand<Result<OrderResponse>>;