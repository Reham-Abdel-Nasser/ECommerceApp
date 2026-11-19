using FluentValidation;

namespace ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;

public class GetDeliveryMethodByIdQueryValidator
    : AbstractValidator<GetDeliveryMethodByIdQuery>
{
    public GetDeliveryMethodByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("DeliveryMethod.Id.Required")
            .WithMessage("Delivery Method id is required");
    }
}
