using ECommerce.Domain.Entities;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.DeliveryMethods.Specifications;

public class DeliveryMethodByIdSpec : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public DeliveryMethodByIdSpec(Guid id)
    {
        Query
            .Where(m => m.Id == id)
            .Select(m => new DeliveryMethodResponse(m.Id, m.Name, m.Description, 
            m.Price, m.EstimatedDeliveryTime, m.IsAvailable, m.DisplayOrder));
    }
}
