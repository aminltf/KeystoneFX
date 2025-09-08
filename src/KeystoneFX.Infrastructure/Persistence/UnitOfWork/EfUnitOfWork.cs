using KeystoneFX.Shared.Kernel.Abstractions.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Persistence.UnitOfWork;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly DbContext _db;
    private IDbContextTransaction? _currentTx;
    private bool _trackChanges = true;

    public EfUnitOfWork(DbContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
        _db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
    }

    // Tracking policy (applies to future queries created from this DbContext)
    public bool TrackChanges
    {
        get => _trackChanges;
        set
        {
            _trackChanges = value;
            _db.ChangeTracker.QueryTrackingBehavior =
                value ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
        }
    }

    // SaveChanges
    public Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess = true,
        CancellationToken ct = default)
        => _db.SaveChangesAsync(acceptAllChangesOnSuccess, ct);

    // Transactions (Begin/Commit/Rollback)
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTx is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        _currentTx = await _db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTx is null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _currentTx.CommitAsync(ct).ConfigureAwait(false);
        await _currentTx.DisposeAsync().ConfigureAwait(false);
        _currentTx = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTx is null)
            throw new InvalidOperationException("No active transaction to roll back.");

        await _currentTx.RollbackAsync(ct).ConfigureAwait(false);
        await _currentTx.DisposeAsync().ConfigureAwait(false);
        _currentTx = null;
    }

    // Execute a delegate inside a (possibly new) transaction
    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default)
    {
        if (operation is null) throw new ArgumentNullException(nameof(operation));

        var ownsTx = _currentTx is null;
        if (ownsTx)
            _currentTx = await _db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

        try
        {
            await operation(ct).ConfigureAwait(false);

            if (ownsTx)
            {
                await _currentTx!.CommitAsync(ct).ConfigureAwait(false);
                await _currentTx.DisposeAsync().ConfigureAwait(false);
                _currentTx = null;
            }
        }
        catch
        {
            if (ownsTx && _currentTx is not null)
            {
                try
                {
                    await _currentTx.RollbackAsync(ct).ConfigureAwait(false);
                }
                finally
                {
                    await _currentTx.DisposeAsync().ConfigureAwait(false);
                    _currentTx = null;
                }
            }
            throw;
        }
    }

    // Cleanup
    public async ValueTask DisposeAsync()
    {
        if (_currentTx is not null)
        {
            await _currentTx.DisposeAsync().ConfigureAwait(false);
            _currentTx = null;
        }
    }
}