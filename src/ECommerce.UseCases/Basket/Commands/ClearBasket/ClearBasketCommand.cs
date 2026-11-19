using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.ClearBasket;

public sealed record ClearBasketCommand(Guid BuyerId) : ICommand<Result<GetBasketResponse>>;
