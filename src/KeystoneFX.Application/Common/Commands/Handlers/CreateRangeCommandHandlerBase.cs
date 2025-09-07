using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using AutoMapper;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class CreateRangeCommandHandlerBase<TCommand, TCreateDto, TEntity, TKey>
    : IRequestHandler<TCommand, IReadOnlyCollection<TKey>>
    where TCommand : ICreateRangeCommand<TCreateDto, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;
    protected readonly IMapper Mapper;

    protected CreateRangeCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        Repo = repo;
        Uow = uow;
        Mapper = mapper;
    }

    public virtual async Task<IReadOnlyCollection<TKey>> Handle(TCommand request, CancellationToken ct)
    {
        var models = request.Models;
        if (models.Count == 0)
            return Array.Empty<TKey>();

        await OnBeforeMapAsync(models, ct).ConfigureAwait(false);

        var entities = new List<TEntity>(models.Count);
        foreach (var dto in models)
        {
            var e = Mapper.Map<TEntity>(dto);
            entities.Add(e);
        }

        await OnBeforeAddRangeAsync(entities, request, ct).ConfigureAwait(false);

        await Repo.AddRangeAsync(entities, ct).ConfigureAwait(false);
        await Uow.SaveChangesAsync(true, ct).ConfigureAwait(false);

        await OnAfterAddRangeAsync(entities, request, ct).ConfigureAwait(false);

        // Return generated keys after SaveChanges
        return entities.Select(x => x.Id).ToArray();
    }

    protected virtual Task OnBeforeMapAsync(IReadOnlyCollection<TCreateDto> dtos, CancellationToken ct)
        => Task.CompletedTask;

    protected virtual Task OnBeforeAddRangeAsync(IReadOnlyCollection<TEntity> entities, TCommand request, CancellationToken ct)
        => Task.CompletedTask;

    protected virtual Task OnAfterAddRangeAsync(IReadOnlyCollection<TEntity> entities, TCommand request, CancellationToken ct)
        => Task.CompletedTask;
}