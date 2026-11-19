using ECommerce.Domain.Enums;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public sealed class Order : BaseEntity
{
    public const int MaxNameLength = 100;
    public const int MaxPhoneLength = 32;
    public const int MaxCountryLength = 100;
    public const int MaxCityLength = 100;
    public const int MaxStreetLength = 200;
    public const int MaxPostalCodeLength = 20;
    public const int MaxDeliveryMethodNameLength = 100;
    public const int MaxDeliveryTimeLength = 100;

    private readonly List<OrderItem> _items = [];

    private Order()
    {
    }

    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Guid DeliveryMethodId { get; private set; }
    public DeliveryMethod DeliveryMethod { get; private set; } = null!;
    public string DeliveryMethodName { get; private set; } = null!;
    public decimal DeliveryMethodPrice { get; private set; }
    public string DeliveryMethodEstimatedTime { get; private set; } = null!;
    public string ShippingRecipientFirstName { get; private set; } = null!;
    public string ShippingRecipientLastName { get; private set; } = null!;
    public string ShippingPhoneNumber { get; private set; } = null!;
    public string ShippingCountry { get; private set; } = null!;
    public string ShippingCity { get; private set; } = null!;
    public string ShippingStreet { get; private set; } = null!;
    public string ShippingPostalCode { get; private set; } = null!;

    //public ShippingAddress ShippingAddress { get; private set; } = null!;

    public decimal SubTotal { get; private set; } // without shipping
    public decimal ShippingCost { get; private set; }
    public decimal Total { get; private set; }

    public string? PaymentIntentId { get; private set; }
    public DateTimeOffset? PaidAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    public static Result<Order> Create(
        Guid id,
        Guid userId,
        DeliveryMethod deliveryMethod,
        UserAddress shippingAddress,
        IReadOnlyList<(Guid ProductId, string ProductName, string PictureUrl, decimal UnitPrice, int Quantity)> basketItems)
    {
        if (id == Guid.Empty)
            return Result<Order>.Failure(OrderErrors.InvalidId);

        if (userId == Guid.Empty)
            return Result<Order>.Failure(OrderErrors.InvalidUserId);

        if (deliveryMethod is null)
            return Result<Order>.Failure(OrderErrors.DeliveryMethodRequired);

        if (!deliveryMethod.IsAvailable)
            return Result<Order>.Failure(OrderErrors.DeliveryMethodUnavailable);

        if (shippingAddress is null)
            return Result<Order>.Failure(OrderErrors.ShippingAddressRequired);

        if (shippingAddress.UserId != userId)
            return Result<Order>.Failure(OrderErrors.ShippingAddressNotOwned);

        if (basketItems is null || basketItems.Count == 0)
            return Result<Order>.Failure(OrderErrors.EmptyBasket);

        //var addressResult = ShippingAddress.Create(shippingAddress.RecipientFirstName,
        //    shippingAddress.RecipientLastName, shippingAddress.PhoneNumber,
        //    shippingAddress.Country, shippingAddress.City, shippingAddress.Street,
        //    shippingAddress.PostalCode);

        //if (addressResult.IsFailure)
        //    return Result<Order>.Failure(addressResult.Error!);

        var order = new Order
        {
            Id = id,
            UserId = userId,
            Status = OrderStatus.Pending,
            DeliveryMethodId = deliveryMethod.Id,
            DeliveryMethodName = deliveryMethod.Name,
            DeliveryMethodPrice = deliveryMethod.Price,
            DeliveryMethodEstimatedTime = deliveryMethod.EstimatedDeliveryTime,
            //ShippingAddress = addressResult.Value!,
            ShippingRecipientFirstName = shippingAddress.RecipientFirstName,
            ShippingRecipientLastName = shippingAddress.RecipientLastName,
            ShippingPhoneNumber = shippingAddress.PhoneNumber,
            ShippingCountry = shippingAddress.Country,
            ShippingCity = shippingAddress.City,
            ShippingStreet = shippingAddress.Street,
            ShippingPostalCode = shippingAddress.PostalCode,
            ShippingCost = deliveryMethod.Price
        };

        foreach (var (ProductId, ProductName, PictureUrl, UnitPrice, Quantity) in basketItems)
        {
            var snapshotResult = ProductItemOrdered.Create(
                ProductId,
                ProductName,
                PictureUrl,
                UnitPrice);

            if (snapshotResult.IsFailure)
                return Result<Order>.Failure(snapshotResult.Error!);

            var itemResult = OrderItem.Create(
                Guid.NewGuid(),
                snapshotResult.Value!,
                Quantity);

            if (itemResult.IsFailure)
                return Result<Order>.Failure(itemResult.Error!);

            var item = itemResult.Value!;
            item.AssignOrder(id);
            order._items.Add(item);
        }

        order.SubTotal = order._items.Sum(i => i.LineTotal);
        order.Total = order.SubTotal + order.ShippingCost;

        return Result<Order>.Success(order);
    }

    public Result Cancel()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.CannotCancel);

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }
    /// <summary>
    /// Transitions the order from Pending to Processing (e.g., once payment is confirmed).
    /// </summary>
    /// <returns></returns>

    public Result Process()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidPaymentState); // Or a dedicated OrderErrors.CannotProcess if preferred.

        Status = OrderStatus.Processing;
        return Result.Success();
    }

    public Result AttachPaymentIntent(string paymentIntentId)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidPaymentState);

        if (string.IsNullOrWhiteSpace(paymentIntentId))
            return Result.Failure(OrderErrors.InvalidPaymentIntent);

        PaymentIntentId = paymentIntentId.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result MarkAsPaid(string paymentIntentId)
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.CannotPayCancelled);

        // Idempotent: already paid with same intent
        if (Status == OrderStatus.Processing
            && PaymentIntentId == paymentIntentId
            && PaidAtUtc is not null)
            return Result.Success();

        if (Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidPaymentState);

        if (!string.IsNullOrWhiteSpace(PaymentIntentId)
            && PaymentIntentId != paymentIntentId)
            return Result.Failure(OrderErrors.PaymentIntentMismatch);

        PaymentIntentId = paymentIntentId;
        PaidAtUtc = DateTimeOffset.UtcNow;
        Status = OrderStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }
}