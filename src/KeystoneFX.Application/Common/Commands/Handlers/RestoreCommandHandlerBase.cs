using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class RestoreCommandHandlerBase<TCommand, TEntity, TKey>
    : IRequestHandler<TCommand, bool>
    where TCommand : IRestoreCommand<TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;

    protected RestoreCommandHandlerBase(
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

        await OnBeforeRestoreAsync(entity, request, ct);

        await Repo.RestoreAsync(request.Id, ct);
        var affected = await Uow.SaveChangesAsync(true, ct);

        await OnAfterRestoreAsync(entity, request, ct);

        return affected > 0;
    }

    protected virtual Task OnNotFoundAsync(TKey id, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeRestoreAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterRestoreAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
}