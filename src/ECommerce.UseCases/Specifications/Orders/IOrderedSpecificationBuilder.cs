using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Orders;

public interface IOrderedSpecificationBuilder<T> : ISpecificationBuilder<T> where T : class
{
    IOrderedSpecificationBuilder<T> ThenBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T> ThenByDescending(Expression<Func<T, object?>> orderExpression);

}

public interface IOrderedSpecificationBuilder<T, TResult>
    : ISpecificationBuilder<T, TResult> where T : class
{
    IOrderedSpecificationBuilder<T, TResult> ThenBy(Expression<Func<T, object?>> orderExpression);

    IOrderedSpecificationBuilder<T, TResult> ThenByDescending(Expression<Func<T, object?>> orderExpression);
}
