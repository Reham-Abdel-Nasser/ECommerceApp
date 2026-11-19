using System.Linq.Expressions;

namespace ECommerce.UseCases.Specifications.Includes;

public interface IIncludableSpecificationBuilder<T, TProperty> : ISpecificationBuilder<T> where T : class
{
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(
        Expression<Func<TProperty, TNext>> navigation);
}
// include(o => o.Customer)     T: Order     TProperty: Customer
// theninclude(c => c.Address)

public interface IIncludableCollectionSpecificationBuilder<T, TElement>
    : ISpecificationBuilder<T> where T : class
{
    IIncludableSpecificationBuilder<T, TNext> ThenInclude<TNext>(
        Expression<Func<TElement, TNext>> navigation);
}
