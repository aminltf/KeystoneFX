using KeystoneFX.Infrastructure.Persistence.Specifications;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Core.Querying.Spec;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Persistence.Repositories;

public class EfReadRepository<TEntity, TKey> : IReadRepository<TEntity, TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    private readonly DbContext _db;
    private readonly DbSet<TEntity> _set;
    private readonly SpecificationEvaluatorOptions _options;

    public EfReadRepository(DbContext db, SpecificationEvaluatorOptions? options = null)
    {
        _db = db;
        _set = _db.Set<TEntity>();
        _options = options ?? new SpecificationEvaluatorOptions();
    }

    public IQueryable<TEntity> Query(bool includeDeleted = false)
        => (includeDeleted ? _set.IgnoreQueryFilters() : _set).AsNoTracking();

    public Task<TEntity?> GetByIdAsync(
        TKey id,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => (includeDeleted ? _set.IgnoreQueryFilters() : _set)
           .AsNoTracking()
           .FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);

    // Spec (Entity)
    public Task<TEntity?> FirstOrDefaultAsync(
        ISpecification<TEntity> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => SpecificationEvaluator
           .GetQuery(_set, spec, includeDeleted, _options)
           .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        ISpecification<TEntity> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => await SpecificationEvaluator
           .GetQuery(_set, spec, includeDeleted, _options)
           .ToListAsync(ct);

    public Task<int> CountAsync(
        ISpecification<TEntity> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => SpecificationEvaluator
           .GetQueryForCountOrAny(_set, spec, includeDeleted, _options)
           .CountAsync(ct);

    public Task<bool> AnyAsync(
        ISpecification<TEntity> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => SpecificationEvaluator
           .GetQueryForCountOrAny(_set, spec, includeDeleted, _options)
           .AnyAsync(ct);

    // Spec (Projection)
    public Task<TResult?> GetAsync<TResult>(
        IProjectedSpecification<TEntity, TResult> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => SpecificationEvaluator
           .GetProjectedQuery(_set, spec, includeDeleted, _options)
           .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        IProjectedSpecification<TEntity, TResult> spec,
        bool includeDeleted = false,
        CancellationToken ct = default)
        => await SpecificationEvaluator
           .GetProjectedQuery(_set, spec, includeDeleted, _options)
           .ToListAsync(ct);
}