using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class SoftDeleteRangeCommandHandlerBase<TCommand, TEntity, TKey>
        : IRequestHandler<TCommand, int>
        where TCommand : ISoftDeleteRangeCommand<TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;

    protected SoftDeleteRangeCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow)
    {
        Repo = repo;
        Uow = uow;
    }

    public virtual async Task<int> Handle(TCommand request, CancellationToken ct)
    {
        var ids = (request.Ids ?? Enumerable.Empty<TKey>()).Distinct().ToArray();
        if (ids.Length == 0) return 0;

        await OnBeforeSoftDeleteRangeAsync(ids, request, ct);

        await Repo.SoftDeleteRangeAsync(ids, ct);
        var affected = await Uow.SaveChangesAsync(true, ct);

        await OnAfterSoftDeleteRangeAsync(ids, affected, request, ct);

        return affected;
    }

    protected virtual Task OnBeforeSoftDeleteRangeAsync(IReadOnlyCollection<TKey> ids, TCommand request, CancellationToken ct)
        => Task.CompletedTask;

    protected virtual Task OnAfterSoftDeleteRangeAsync(IReadOnlyCollection<TKey> ids, int affectedRows, TCommand request, CancellationToken ct)
        => Task.CompletedTask;
}