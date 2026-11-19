using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging.Abstractions;
using ECommerce.UseCases.Orders.Dtos;

namespace ECommerce.UseCases.Orders.Commands.CreateOrderPayment;

public sealed record CreateOrderPaymentCommand(Guid OrderId)
    : ICommand<Result<PaymentClientSecretResponse>>;