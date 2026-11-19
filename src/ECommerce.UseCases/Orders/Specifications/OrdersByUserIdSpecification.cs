using ECommerce.Domain.Entities;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Orders.Specifications;

public sealed class OrdersByUserIdSpecification : Specification<Order>
{
    public OrdersByUserIdSpecification(Guid userId, int? pageNumber = null, int? pageSize = null)
    {
        var query = Query
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAt)
            .AsNoTracking();

        if(pageNumber.HasValue && pageSize.HasValue)
        {
            var skip = (pageNumber.Value - 1) * pageSize.Value;

            query
                .Include(order => order.Items)
                .Skip(skip)
                .Take(pageSize.Value);
        }
    }
}
