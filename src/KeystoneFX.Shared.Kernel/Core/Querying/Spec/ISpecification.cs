using System.Linq.Expressions;

namespace KeystoneFX.Shared.Kernel.Core.Querying.Spec;

public interface ISpecification<TEntity>
{
    Expression<Func<TEntity, bool>>? Criteria { get; }

    IReadOnlyList<Expression<Func<TEntity, object>>> Includes { get; }

    IReadOnlyList<OrderExpression<TEntity>> OrderBy { get; }

    int? Skip { get; }

    int? Take { get; }

    bool AsNoTracking { get; }
}

public interface IProjectedSpecification<TEntity, TResult> : ISpecification<TEntity>
{
    Expression<Func<TEntity, TResult>> Selector { get; }
}