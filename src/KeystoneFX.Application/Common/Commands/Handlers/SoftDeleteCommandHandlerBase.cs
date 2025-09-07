using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class SoftDeleteCommandHandlerBase<TCommand, TEntity, TKey>
        : IRequestHandler<TCommand, bool>
        where TCommand : ISoftDeleteCommand<TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;

    protected SoftDeleteCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow)
    {
        Repo = repo;
        Uow = uow;
    }

    public virtual async Task<bool> Handle(TCommand request, CancellationToken ct)
    {
        var entity = await Repo.LoadByIdAsync(request.Id, includeDeleted: true, ct);
        if (entity is null)
        {
            await OnNotFoundAsync(request.Id, ct);
            return false;
        }

        await OnBeforeSoftDeleteAsync(entity, request, ct);

        await Repo.SoftDeleteAsync(request.Id, ct);
        var affected = await Uow.SaveChangesAsync(true, ct);

        await OnAfterSoftDeleteAsync(entity, request, ct);

        return affected > 0;
    }

    protected virtual Task OnNotFoundAsync(TKey id, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeSoftDeleteAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterSoftDeleteAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
}