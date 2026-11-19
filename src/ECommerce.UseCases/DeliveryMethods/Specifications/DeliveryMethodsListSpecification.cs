using ECommerce.Domain.Entities;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.DeliveryMethods.Specifications;

public sealed class DeliveryMethodsListSpecification 
    : Specification<DeliveryMethod, DeliveryMethodResponse>
{
    public DeliveryMethodsListSpecification(bool availableOnly = false, string? search = null)
    {
        var query = Query;

        if (availableOnly)
            query.Where(method => method.IsAvailable);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query.Where(method => 
                method.Name.Contains(term) ||
                (method.Description != null && method.Description.Contains(term)) ||
                method.EstimatedDeliveryTime.Contains(term));
        }

        query
            .OrderBy(method => method.DisplayOrder)
            .ThenBy(method => method.Name)
            .Select(method => new DeliveryMethodResponse(
                method.Id,
                method.Name,
                method.Description,
                method.Price,
                method.EstimatedDeliveryTime,
                method.IsAvailable,
                method.DisplayOrder));
    }
}

public sealed class DeliveryMethodByNameSpecification : Specification<DeliveryMethod>
{
    public DeliveryMethodByNameSpecification(string name, Guid? excludeId = null)
    {
        var trimmedName = name.Trim();
        Query.Where(method => method.Name == trimmedName);

        if (excludeId.HasValue)
            Query.Where(method => method.Id != excludeId.Value);
    }
}