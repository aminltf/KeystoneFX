using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Core.Querying.Spec;

namespace KeystoneFX.Shared.Kernel.Abstractions.Data;

public interface IReadRepository<TEntity, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> Query(bool includeDeleted = false);

    // Convenience
    Task<TEntity?> GetByIdAsync(
        TKey id,
        bool includeDeleted = false,
        CancellationToken ct = default);

    // Specification
    #region Spec-based
    Task<TEntity?> FirstOrDefaultAsync(
            ISpecification<TEntity> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);

        Task<IReadOnlyList<TEntity>> ListAsync(
            ISpecification<TEntity> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);

        Task<int> CountAsync(
            ISpecification<TEntity> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);

        Task<bool> AnyAsync(
            ISpecification<TEntity> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);

    // Spec-based Projection Server-Side
    Task<TResult?> GetAsync<TResult>(
            IProjectedSpecification<TEntity, TResult> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);

        Task<IReadOnlyList<TResult>> ListAsync<TResult>(
            IProjectedSpecification<TEntity, TResult> spec,
            bool includeDeleted = false,
            CancellationToken ct = default);
    #endregion
}