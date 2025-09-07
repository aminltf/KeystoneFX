using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class DeleteCommandHandlerBase<TCommand, TEntity, TKey>
        : IRequestHandler<TCommand, Unit>
        where TCommand : IDeleteCommand<TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;

    protected DeleteCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow)
    {
        Repo = repo;
        Uow = uow;
    }

    public virtual async Task<Unit> Handle(TCommand request, CancellationToken ct)
    {
        var entity = await Repo.LoadByIdAsync(request.Id, includeDeleted: true, ct);
        if (entity is null)
        {
            await OnNotFoundAsync(request.Id, ct);
            return Unit.Value;
        }

        await OnBeforeRemoveAsync(entity, request, ct);

        await Repo.RemoveAsync(request.Id, ct);
        await Uow.SaveChangesAsync(true, ct);

        await OnAfterRemoveAsync(entity, request, ct);

        return Unit.Value;
    }

    protected virtual Task OnNotFoundAsync(TKey id, CancellationToken ct) => Task.CompletedTask;

    protected virtual Task OnBeforeRemoveAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;

    protected virtual Task OnAfterRemoveAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
}