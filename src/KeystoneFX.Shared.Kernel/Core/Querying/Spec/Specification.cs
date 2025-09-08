using System.Linq.Expressions;

namespace KeystoneFX.Shared.Kernel.Core.Querying.Spec;

public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    private readonly List<Expression<Func<TEntity, object>>> _includes = [];
    private readonly List<OrderExpression<TEntity>> _orderBy = [];

    public Expression<Func<TEntity, bool>>? Criteria { get; protected set; }
    public IReadOnlyList<Expression<Func<TEntity, object>>> Includes => _includes;
    public IReadOnlyList<OrderExpression<TEntity>> OrderBy => _orderBy;
    public int? Skip { get; protected set; }
    public int? Take { get; protected set; }
    public bool AsNoTracking { get; protected set; } = true;

    // Protected helpers
    protected void Where(Expression<Func<TEntity, bool>> predicate)
            => Criteria = Criteria is null ? predicate : Criteria.And(predicate);

    protected void OrWhere(Expression<Func<TEntity, bool>> predicate)
        => Criteria = Criteria is null ? predicate : Criteria.Or(predicate);

    protected void Include(Expression<Func<TEntity, object>> include) => _includes.Add(include);

    protected void OrderByAsc(Expression<Func<TEntity, object>> keySelector)
        => _orderBy.Add(new OrderExpression<TEntity>(keySelector, Descending: false));

    protected void OrderByDesc(Expression<Func<TEntity, object>> keySelector)
        => _orderBy.Add(new OrderExpression<TEntity>(keySelector, Descending: true));

    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip >= 0 ? skip : throw new ArgumentOutOfRangeException(nameof(skip));
        Take = take > 0 ? take : throw new ArgumentOutOfRangeException(nameof(take));
    }

    protected void AsTracking() => AsNoTracking = false;
    protected void AsReadOnly() => AsNoTracking = true;
}

public abstract class ProjectedSpecification<TEntity, TResult>
        : Specification<TEntity>, IProjectedSpecification<TEntity, TResult>
{
    public Expression<Func<TEntity, TResult>> Selector { get; protected set; } = default!;

    protected void Select(Expression<Func<TEntity, TResult>> selector)
        => Selector = selector ?? throw new ArgumentNullException(nameof(selector));
}