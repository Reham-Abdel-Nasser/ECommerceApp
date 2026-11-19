using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductBrandConfiguration : IEntityTypeConfiguration<ProductBrand>
{
    public void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(b => b.Name)
            .IsUnique();
    }
}