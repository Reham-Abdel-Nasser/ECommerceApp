using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

// orderby(p => p.Price)
public sealed record OrderExpressionInfo<T>(Expression<Func<T, object?>> KeySelector,
    OrderType OrderType) where T : class;

public enum OrderType
{
    OrderBy,
    OrderByDescending,
    ThenBy,
    ThenByDescending
}