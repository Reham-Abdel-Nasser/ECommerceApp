using FluentValidation;

namespace ECommerce.UseCases.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ShippingAddressId)
            .NotEmpty()
            .WithErrorCode("Order.ShippingAddressId.Required")
            .WithMessage("Shipping Address id is required.");

        RuleFor(x => x.DeliveryMethodId)
            .NotEmpty()
            .WithErrorCode("Order.DeliveryMethodId.Required")
            .WithMessage("Delivery method id is required.");
    }
}
