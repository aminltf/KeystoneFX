using KeystoneFX.Shared.Kernel.Core.Querying.Spec;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator
{
    // Entity Full
    public static IQueryable<TEntity> GetQuery<TEntity>(
            IQueryable<TEntity> source,
            ISpecification<TEntity> spec,
            bool includeDeleted,
            SpecificationEvaluatorOptions? options = null)
            where TEntity : class
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        options ??= new SpecificationEvaluatorOptions();

        var q = includeDeleted ? source.IgnoreQueryFilters() : source;
        q = spec.AsNoTracking ? q.AsNoTracking() : q;

        if (spec.Criteria is not null)
            q = q.Where(spec.Criteria);

        if (spec.Includes.Count > 0)
        {
            if (options.UseSplitQueryForMultipleIncludes && spec.Includes.Count > 1)
                q = q.AsSplitQuery();

            foreach (var inc in spec.Includes)
                q = q.Include(inc);
        }

        if (spec.OrderBy.Count > 0)
            q = ApplyOrdering(q, spec.OrderBy);

        if (spec.Skip is int s) q = q.Skip(s);
        if (spec.Take is int t) q = q.Take(t);

        return q;
    }

    // Entity Base just for Count/Any
    public static IQueryable<TEntity> GetQueryForCountOrAny<TEntity>(
        IQueryable<TEntity> source,
        ISpecification<TEntity> spec,
        bool includeDeleted,
        SpecificationEvaluatorOptions? options = null)
        where TEntity : class
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        options ??= new SpecificationEvaluatorOptions();

        var q = includeDeleted ? source.IgnoreQueryFilters() : source;
        q = spec.AsNoTracking ? q.AsNoTracking() : q;

        if (spec.Criteria is not null)
            q = q.Where(spec.Criteria);

        return q;
    }

    // Projection (DTO/Result)
    public static IQueryable<TResult> GetProjectedQuery<TEntity, TResult>(
        IQueryable<TEntity> source,
        IProjectedSpecification<TEntity, TResult> spec,
        bool includeDeleted,
        SpecificationEvaluatorOptions? options = null)
        where TEntity : class
    {
        if (spec is null) throw new ArgumentNullException(nameof(spec));
        options ??= new SpecificationEvaluatorOptions();

        var q = includeDeleted ? source.IgnoreQueryFilters() : source;
        q = spec.AsNoTracking ? q.AsNoTracking() : q;

        if (spec.Criteria is not null)
            q = q.Where(spec.Criteria);

        if (options.ApplyIncludesInProjection && spec.Includes.Count > 0)
        {
            if (options.UseSplitQueryForMultipleIncludes && spec.Includes.Count > 1)
                q = q.AsSplitQuery();

            foreach (var inc in spec.Includes)
                q = q.Include(inc);
        }

        if (spec.OrderBy.Count > 0)
            q = ApplyOrdering(q, spec.OrderBy);

        if (spec.Skip is int s) q = q.Skip(s);
        if (spec.Take is int t) q = q.Take(t);

        return q.Select(spec.Selector);
    }

    // Helpers
    private static IQueryable<TEntity> ApplyOrdering<TEntity>(
        IQueryable<TEntity> query,
        IReadOnlyList<OrderExpression<TEntity>> orderings)
    {
        if (orderings.Count == 0) return query;

        IOrderedQueryable<TEntity>? ordered = null;
        for (int i = 0; i < orderings.Count; i++)
        {
            var ord = orderings[i];
            ordered = i == 0
                ? ord.Descending ? query.OrderByDescending(ord.KeySelector)
                                  : query.OrderBy(ord.KeySelector)
                : ord.Descending ? ordered!.ThenByDescending(ord.KeySelector)
                                  : ordered!.ThenBy(ord.KeySelector);
        }
        return ordered ?? query;
    }
}
