using ECommerce.Domain.Entities;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Orders.Specifications;

public sealed class UserAddressByUserIdSpecification 
    : Specification<UserAddress, UserAddressResponse>
{
    public UserAddressByUserIdSpecification(Guid userId)
    {
        Query
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.Label)
            .Select(a => new UserAddressResponse(
                a.Id,
                a.Label,
                a.RecipientFirstName,
                a.RecipientLastName,
                a.PhoneNumber,
                a.Country,
                a.City,
                a.Street,
                a.PostalCode,
                a.IsDefault));
    }
}