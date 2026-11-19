using ECommerce.Domain.Specifications;
using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Orders;

public sealed class OrderedSpecificationBuilder<T> : SpecificationBuilder<T>,
    IOrderedSpecificationBuilder<T> where T : class
{
    internal OrderedSpecificationBuilder(Specification<T> specification)
        : base(specification)
    {

    }
    public IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenBy));
        return this;
    }

    public IOrderedSpecificationBuilder<T> ThenByDescending(Expression<Func<T, object?>> orderExpression)
    {
        Specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenByDescending));
        return this;
    }
}

public sealed class OrderedSpecificationBuilder<T, TResult>
    : SpecificationBuilder<T, TResult>, IOrderedSpecificationBuilder<T, TResult> where T : class
{
    private readonly Specification<T, TResult> _specification;
    internal OrderedSpecificationBuilder(Specification<T, TResult> specification)
        : base(specification)
    {
        _specification = specification;
    }

    public IOrderedSpecificationBuilder<T, TResult> ThenBy(Expression<Func<T, object?>> orderExpression)
    {
        _specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenBy));
        return this;
    }

    public IOrderedSpecificationBuilder<T, TResult> ThenByDescending(Expression<Func<T, object?>> orderExpression)
    {
        _specification.AddOrder(new OrderExpressionInfo<T>(orderExpression, OrderType.ThenByDescending));
        return this;
    }
}