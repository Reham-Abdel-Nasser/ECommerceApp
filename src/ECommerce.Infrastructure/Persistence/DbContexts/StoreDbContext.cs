using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.DbContexts
{
    public class StoreDbContext(DbContextOptions<StoreDbContext> options) : DbContext(options)
    {
        public DbSet<ProductType> Types => Set<ProductType>();
        public DbSet<ProductBrand> Brands => Set<ProductBrand>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly,
                type => type.Namespace == "ECommerce.Infrastructure.Persistence.Configurations");

            base.OnModelCreating(modelBuilder);

        }
    }
}