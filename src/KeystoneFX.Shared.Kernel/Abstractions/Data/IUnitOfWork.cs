namespace KeystoneFX.Shared.Kernel.Abstractions.Data;

public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess = true,
        CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);

    Task CommitTransactionAsync(CancellationToken ct = default);

    Task RollbackTransactionAsync(CancellationToken ct = default);

    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default);

    bool TrackChanges { get; set; }
}