using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.UseCases.DeliveryMethods.Commands.CreateDeliveryMethod;

public sealed class CreateDeliveryMethodCommandValidator 
    : AbstractValidator<CreateDeliveryMethodCommand>
{
    public CreateDeliveryMethodCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DeliveryMethod.MaxNameLength);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.EstimatedDeliveryTime)
            .NotEmpty()
            .MaximumLength(DeliveryMethod.MaxDeliveryTimeLength);

        RuleFor(x => x.Description)
            .MaximumLength(DeliveryMethod.MaxDescriptionLength)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}
