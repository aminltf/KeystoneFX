using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Abstractions.Data;

public interface IWriteRepository<TEntity, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity?> LoadByIdAsync(
        TKey id,
        bool includeDeleted = false,
        CancellationToken ct = default);

    Task<IReadOnlyList<TEntity>> LoadByIdsAsync(
            IEnumerable<TKey> ids,
            bool includeDeleted = false,
            CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    Task UpdateAsync(TEntity entity, CancellationToken ct = default);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

    Task RemoveAsync(TKey id, CancellationToken ct = default);

    Task RemoveRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default);

    Task SoftDeleteAsync(TKey id, CancellationToken ct = default);

    Task SoftDeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default);

    Task RestoreAsync(TKey id, CancellationToken ct = default);

    Task RestoreRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default);

    void Attach(TEntity entity);

    void Detach(TEntity entity);
}