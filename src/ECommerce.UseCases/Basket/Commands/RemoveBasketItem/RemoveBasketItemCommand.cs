using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging.Abstractions;

namespace ECommerce.UseCases.Basket.Commands.RemoveBasketItem;

public sealed record RemoveBasketItemCommand(Guid BuyerId, Guid ProductId)
    : ICommand<Result<GetBasketResponse>>;
