using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.PictureUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.HasOne(p => p.ProductBrand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.ProductBrandId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.ProductType)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.ProductTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.HasIndex(p => p.Price)
           .IsUnique();
    }
}