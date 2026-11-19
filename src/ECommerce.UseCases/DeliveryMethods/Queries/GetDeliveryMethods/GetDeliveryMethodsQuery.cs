using ECommerce.Domain.Shared;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;

public sealed record GetDeliveryMethodsQuery(
    bool AvailableOnly = true, string? Search = null) : IQuery<Result<IReadOnlyList<DeliveryMethodResponse>>>;