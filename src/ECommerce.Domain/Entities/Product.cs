using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;

namespace ECommerce.Domain.Entities;

public class Product : BaseEntity
{
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 1000;
    public const int MaxPictureUrlLength = 500;

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    // Cloud => Azure blob storage , Cloudinary
    public string PictureUrl { get; private set; } = null!;

    public decimal Price { get; private set; }
    public Guid ProductBrandId { get; private set; }
    public ProductBrand ProductBrand { get; private set; } = null!;

    public Guid ProductTypeId { get; private set; }
    public ProductType ProductType { get; private set; } = null!;

    private Product()
    {
    }

    public static Result<Product> Create(
        Guid id,
        string name,
        string description,
        string pictureUrl,
        decimal price,
        Guid productBrandId,
        Guid productTypeId)
    {
        if (id == Guid.Empty)
            return Result<Product>.Failure(ProductErrors.InvalidId);

        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure(ProductErrors.InvalidName);

        if (name.Length > MaxNameLength)
            return Result<Product>.Failure(ProductErrors.NameTooLong);

        if (string.IsNullOrWhiteSpace(description))
            return Result<Product>.Failure(ProductErrors.InvalidDescription);

        if (description.Length > MaxDescriptionLength)
            return Result<Product>.Failure(ProductErrors.DescriptionTooLong);

        if (string.IsNullOrWhiteSpace(pictureUrl))
            return Result<Product>.Failure(ProductErrors.InvalidPictureUrl);

        if (pictureUrl.Length > MaxPictureUrlLength)
            return Result<Product>.Failure(ProductErrors.PictureUrlTooLong);

        if (price <= 0)
            return Result<Product>.Failure(ProductErrors.InvalidPrice);

        if (productBrandId == Guid.Empty)
            return Result<Product>.Failure(ProductErrors.InvalidBrand);

        if (productTypeId == Guid.Empty)
            return Result<Product>.Failure(ProductErrors.InvalidType);

        var product = new Product
        {
            Id = id,
            Name = name.Trim(),
            Description = description.Trim(),
            PictureUrl = pictureUrl.Trim(),
            Price = price,
            ProductBrandId = productBrandId,
            ProductTypeId = productTypeId
        };

        return Result<Product>.Success(product);
    }

    public Result Update(
        string name,
        string description,
        string pictureUrl,
        decimal price,
        Guid productBrandId,
        Guid productTypeId)
    {
        var nameResult = SetName(name);

        if (nameResult.IsFailure)
            return nameResult;

        var descriptionResult = SetDescription(description);
        if (descriptionResult.IsFailure)
            return descriptionResult;

        var pictureUrlResult = SetPictureUrl(pictureUrl);
        if (pictureUrlResult.IsFailure)
            return pictureUrlResult;

        var priceResult = SetPrice(price);
        if (priceResult.IsFailure)
            return priceResult;

        var productBrandResult = SetBrand(productBrandId);
        if (productBrandResult.IsFailure)
            return productBrandResult;

        var productTypeResult = SetType(productTypeId);
        if (productTypeResult.IsFailure)
            return productTypeResult;

        return Result.Success();

    }

    private Result SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
           return Result.Failure(ProductErrors.InvalidName);

        if (name.Length > MaxNameLength)
            return Result.Failure(ProductErrors.NameTooLong);

        Name = name.Trim();

        return Result.Success();
    }

    private Result SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure(ProductErrors.InvalidDescription);

        if (description.Length > MaxDescriptionLength)
            return Result.Failure(ProductErrors.DescriptionTooLong);

        Description = description.Trim();

        return Result.Success();
    }

    private Result SetPictureUrl(string pictureUrl)
    {
        if (string.IsNullOrWhiteSpace(pictureUrl))
            return Result.Failure(ProductErrors.InvalidPictureUrl);

        if (pictureUrl.Length > MaxPictureUrlLength)
            return Result.Failure(ProductErrors.PictureUrlTooLong);

        PictureUrl = pictureUrl.Trim();

        return Result.Success();
    }

    private Result SetPrice(decimal price)
    {
        if (price <= 0)
            return Result.Failure(ProductErrors.InvalidPrice);

        Price = price;

        return Result.Success();
    }

    private Result SetBrand(Guid productBrandId)
    {
        if (productBrandId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidBrand);

        ProductBrandId = productBrandId;

        return Result.Success();
    }

    private Result SetType(Guid productTypeId)
    {
        if (productTypeId == Guid.Empty)
            return Result.Failure(ProductErrors.InvalidType);

        ProductTypeId = productTypeId;

        return Result.Success();
    }

}
