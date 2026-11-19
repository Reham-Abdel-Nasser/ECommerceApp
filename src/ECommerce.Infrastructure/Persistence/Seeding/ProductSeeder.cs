
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class ProductSeeder(StoreDbContext dbContext) : IDataSeeder
{
    public int Order => 4;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.Products.AnyAsync(ct))
            return;

        var results = new[]
        {
            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                "Classic T-Shirt",
                "Comfortable cotton T-shirt for everyday wear.",
                "https://placehold.co/600x600?text=Classic+T-Shirt",
                25.00m,
                Guid.Parse("11111111-1111-1111-1111-111111111111"), // H&M
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")  // Tops
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000002"),
                "Slim Fit Jeans",
                "Modern slim-fit jeans suitable for casual outfits.",
                "https://placehold.co/600x600?text=Slim+Fit+Jeans",
                45.00m,
                Guid.Parse("22222222-2222-2222-2222-222222222222"), // ZARA
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")  // Bottoms
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000003"),
                "Sports Jacket",
                "Lightweight sports jacket for outdoor activities.",
                "https://placehold.co/600x600?text=Sports+Jacket",
                85.00m,
                Guid.Parse("33333333-3333-3333-3333-333333333333"), // Nike
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")  // Outerwear & Dresses
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                "Casual Hoodie",
                "Soft casual hoodie for everyday comfort.",
                "https://placehold.co/600x600?text=Casual+Hoodie",
                40.00m,
                Guid.Parse("44444444-4444-4444-4444-444444444444"), // Activ
                Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")  // Tops
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000005"),
                "Summer Dress",
                "Lightweight dress designed for warm-weather outings.",
                "https://placehold.co/600x600?text=Summer+Dress",
                55.00m,
                Guid.Parse("55555555-5555-5555-5555-555555555555"), // Mango
                Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")  // Outerwear & Dresses
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000006"),
                "Denim Jeans",
                "Classic denim jeans for everyday styling.",
                "https://placehold.co/600x600?text=Denim+Jeans",
                60.00m,
                Guid.Parse("66666666-6666-6666-6666-666666666666"), // Levis
                Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")  // Bottoms
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000007"),
                "Leather Belt",
                "Versatile belt to complement casual and formal outfits.",
                "https://placehold.co/600x600?text=Leather+Belt",
                20.00m,
                Guid.Parse("22222222-2222-2222-2222-222222222222"), // ZARA
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")  // Accessories & Footwear
            ),

            Product.Create(
                Guid.Parse("10000000-0000-0000-0000-000000000008"),
                "Running Shoes",
                "Comfortable running shoes for daily training.",
                "https://placehold.co/600x600?text=Running+Shoes",
                75.00m,
                Guid.Parse("33333333-3333-3333-3333-333333333333"), // Nike
                Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd")  // Accessories & Footwear
            )
        };

        var failedResult = results.FirstOrDefault(x => x.IsFailure);

        if (failedResult is not null)
        {
            throw new InvalidOperationException(
                $"Product seeding failed: {failedResult.Error?.Code} - " +
                $"{failedResult.Error?.Message}");
        }

        var products = results
            .Select(x => x.Value!)
            .ToArray();

        await dbContext.Products.AddRangeAsync(products, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}