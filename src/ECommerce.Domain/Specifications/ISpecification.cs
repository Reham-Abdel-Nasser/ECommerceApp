using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;

public interface ISpecification<T> where T : class
{
    // Where(product => product.Price > 100)
    IReadOnlyList<Expression<Func<T, bool>>> WhereExpressions { get; }

    // include(p => p.Brand)
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }

    // then include

    IReadOnlyList<IncludeExpressionInfo> IncludeExpressions { get; }

    // order
    IReadOnlyList<OrderExpressionInfo<T>> OrderExpressions { get; }

    int? Skip { get; }
    int? Take { get; }

    bool IsPagingEnabled { get; }

    bool IsTrackingEnabled { get; }

}

// select(p => new {})
public interface ISpecification<T, TResult> : ISpecification<T> where T : class
{
    Expression<Func<T, TResult>>? Selector { get; }
    Expression<Func<T, IEnumerable<TResult>>>? SelectorMany { get; }
}