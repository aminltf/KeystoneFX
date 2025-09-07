using System.Linq.Expressions;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Abstractions.Data;

public interface IReadRepository<TEntity, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> Query(bool includeDeleted = false);

    Task<TEntity?> GetByIdAsync(
        TKey id,
        bool includeDeleted = false,
        CancellationToken ct = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);

    Task<IReadOnlyList<TEntity>> GetAllDeletedAsync(CancellationToken ct = default);

    Task<IReadOnlyList<TEntity>> FindAllAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDeleted = false,
        CancellationToken ct = default);

    Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDeleted = false,
        CancellationToken ct = default,
        params Expression<Func<TEntity, object>>[] includes);

    Task<TEntity?> SingleOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDeleted = false,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool includeDeleted = false,
        CancellationToken ct = default);

    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        bool includeDeleted = false,
        CancellationToken ct = default);
}