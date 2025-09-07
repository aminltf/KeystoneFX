using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using AutoMapper;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class UpdateRangeCommandHandlerBase<TCommand, TUpdateDto, TEntity, TKey>
        : IRequestHandler<TCommand, int>
        where TCommand : IUpdateRangeCommand<TUpdateDto, TKey>
        where TUpdateDto : IHasId<TKey>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;
    protected readonly IMapper Mapper;

    protected UpdateRangeCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        Repo = repo;
        Uow = uow;
        Mapper = mapper;
    }

    public virtual async Task<int> Handle(TCommand request, CancellationToken ct)
    {
        var models = request.Models;
        if (models.Count == 0) return 0;

        var ids = models.Select(m => m.Id).Distinct().ToArray();

        var entities = await Repo.LoadByIdsAsync(ids, includeDeleted: false, ct);
        var byId = entities.ToDictionary(e => e.Id);

        var missing = ids.Where(id => !byId.ContainsKey(id)).ToArray();
        if (missing.Length > 0)
            await OnMissingIdsAsync(missing, request, ct);

        await OnBeforeMapRangeAsync(entities, models, request, ct);

        var toUpdate = new List<TEntity>(entities.Count);
        foreach (var dto in models)
        {
            if (!byId.TryGetValue(dto.Id, out var entity))
                continue;

            await OnBeforeMapItemAsync(entity, dto, request, ct);
            Mapper.Map(dto, entity);
            toUpdate.Add(entity);
        }

        if (toUpdate.Count == 0) return 0;

        await OnBeforeUpdateRangeAsync(toUpdate, request, ct);
        await Repo.UpdateRangeAsync(toUpdate, ct);
        var affected = await Uow.SaveChangesAsync(true, ct);
        await OnAfterUpdateRangeAsync(toUpdate, affected, request, ct);

        return affected;
    }

    // Hooks
    protected virtual Task OnMissingIdsAsync(IReadOnlyCollection<TKey> missingIds, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeMapRangeAsync(IReadOnlyCollection<TEntity> entities, IReadOnlyCollection<TUpdateDto> dtos, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeMapItemAsync(TEntity entity, TUpdateDto dto, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeUpdateRangeAsync(IReadOnlyCollection<TEntity> entities, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterUpdateRangeAsync(IReadOnlyCollection<TEntity> entities, int affectedRows, TCommand request, CancellationToken ct) => Task.CompletedTask;
}