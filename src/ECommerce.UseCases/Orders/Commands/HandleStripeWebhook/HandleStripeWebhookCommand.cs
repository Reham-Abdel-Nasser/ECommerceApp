using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Orders.Commands.HandleStripeWebhook;

public sealed record HandleStripeWebhookCommand(
    string Payload,
    string Signature) : ICommand<Result>;