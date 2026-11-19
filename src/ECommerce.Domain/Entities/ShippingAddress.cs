using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public class ShippingAddress
{
    private ShippingAddress() { }
    public string RecipientFirstName { get; private set; } = null!;
    public string RecipientLastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public string City { get; private set; } = null!;
    public string Street { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;

    public static Result<ShippingAddress> Create(
            string recipientFirstName,
            string recipientLastName,
            string phoneNumber,
            string country,
            string city,
            string street,
            string postalCode
        )
    {
        if (string.IsNullOrWhiteSpace(recipientFirstName))
            return Result<ShippingAddress>.Failure(OrderErrors.RecipientFirstNameRequired);

        if (string.IsNullOrWhiteSpace(recipientLastName))
            return Result<ShippingAddress>.Failure(OrderErrors.RecipientLastNameRequired);

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<ShippingAddress>.Failure(OrderErrors.PhoneNumberRequired);

        if (string.IsNullOrWhiteSpace(country))
            return Result<ShippingAddress>.Failure(OrderErrors.CountryRequired);

        if (string.IsNullOrWhiteSpace(city))
            return Result<ShippingAddress>.Failure(OrderErrors.CityRequired);

        if (string.IsNullOrWhiteSpace(street))
            return Result<ShippingAddress>.Failure(OrderErrors.StreetRequired);
       
        if(string.IsNullOrWhiteSpace(postalCode))
            return Result<ShippingAddress>.Failure(OrderErrors.PostalCodeRequired);


        return Result<ShippingAddress>.Success(new ShippingAddress
        {
            RecipientFirstName = recipientFirstName.Trim(),
            RecipientLastName = recipientLastName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            Country = country.Trim(),
            City = city.Trim(),
            Street = street.Trim(),
            PostalCode = postalCode.Trim(),
        });
    }


    public static Result<ShippingAddress> FromUserAddress(UserAddress address)
    {
        if(address is null)
            return Result<ShippingAddress>.Failure(OrderErrors.ShippingAddressRequired);

        return Create(
            address.RecipientFirstName, address.RecipientLastName,
            address.PhoneNumber, address.Country, address.City,
            address.Street, address.PostalCode);
    }

}
