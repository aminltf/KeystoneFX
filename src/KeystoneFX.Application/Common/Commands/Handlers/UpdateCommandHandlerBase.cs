using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using AutoMapper;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class UpdateCommandHandlerBase<TCommand, TUpdateDto, TEntity, TKey>
        : IRequestHandler<TCommand, bool>
        where TCommand : IUpdateCommand<TUpdateDto, TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;
    protected readonly IMapper Mapper;

    protected UpdateCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        Repo = repo;
        Uow = uow;
        Mapper = mapper;
    }

    public virtual async Task<bool> Handle(TCommand request, CancellationToken ct)
    {
        var entity = await Repo.LoadByIdAsync(request.Id, includeDeleted: false, ct);
        if (entity is null)
        {
            await OnNotFoundAsync(request.Id, ct);
            return false;
        }

        await OnBeforeMapAsync(entity, request.Model, request, ct);

        // Map DTO onto tracked entity
        Mapper.Map(request.Model, entity);

        await OnBeforeUpdateAsync(entity, request, ct);

        await Repo.UpdateAsync(entity, ct);
        var affected = await Uow.SaveChangesAsync(true, ct);

        await OnAfterUpdateAsync(entity, affected, request, ct);

        return affected > 0;
    }

    // Hooks for customization
    protected virtual Task OnNotFoundAsync(TKey id, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeMapAsync(TEntity entity, TUpdateDto dto, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeUpdateAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterUpdateAsync(TEntity entity, int affectedRows, TCommand request, CancellationToken ct) => Task.CompletedTask;
}