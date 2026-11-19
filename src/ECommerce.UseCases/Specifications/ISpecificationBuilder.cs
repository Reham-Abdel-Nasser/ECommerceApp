using ECommerce.UseCases.Specifications.Includes;
using ECommerce.UseCases.Specifications.Orders;
using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications;

public interface ISpecificationBuilder<T> where T : class
{
    ISpecificationBuilder<T> Where(Expression<Func<T, bool>> predicate);

    // Ordering

    IOrderedSpecificationBuilder<T> OrderBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T> OrderByDescending(Expression<Func<T, object?>> orderExpression);

    // Includes (then include)
    IIncludableSpecificationBuilder<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> navigation);
    IIncludableCollectionSpecificationBuilder<T, TElement> Include<TElement>(
        Expression<Func<T, ICollection<TElement>>> navigation);

    ISpecificationBuilder<T> Skip(int skip);
    ISpecificationBuilder<T> Take(int take);

    ISpecificationBuilder<T> AsNoTracking();
    ISpecificationBuilder<T> AsTracking();
}

public interface ISpecificationBuilder<T, TResult> where T : class
{
    ISpecificationBuilder<T, TResult> Where(Expression<Func<T, bool>> predicate);

    // Ordering

    IOrderedSpecificationBuilder<T, TResult> OrderBy(Expression<Func<T, object?>> orderExpression);
    IOrderedSpecificationBuilder<T, TResult> OrderByDescending(Expression<Func<T, object?>> orderExpression);

    // Includes (then include)

    ISpecificationBuilder<T, TResult> Skip(int skip);
    ISpecificationBuilder<T, TResult> Take(int take);

    ISpecificationBuilder<T, TResult> AsNoTracking();
    ISpecificationBuilder<T, TResult> AsTracking();

    ISpecificationBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector);

    ISpecificationBuilder<T, TResult> SelectMany(Expression<Func<T, IEnumerable<TResult>>> selector);

}