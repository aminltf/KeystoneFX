using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using AutoMapper;
using MediatR;

namespace KeystoneFX.Application.Common.Commands.Handlers;

public abstract class CreateCommandHandlerBase<TCommand, TCreateDto, TEntity, TKey>
    : IRequestHandler<TCommand, TKey>
    where TCommand : ICreateCommand<TCreateDto, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly IWriteRepository<TEntity, TKey> Repo;
    protected readonly IUnitOfWork Uow;
    protected readonly IMapper Mapper;

    protected CreateCommandHandlerBase(
        IWriteRepository<TEntity, TKey> repo,
        IUnitOfWork uow,
        IMapper mapper)
    {
        Repo = repo;
        Uow = uow;
        Mapper = mapper;
    }

    public virtual async Task<TKey> Handle(TCommand request, CancellationToken ct)
    {
        await OnBeforeMapAsync(request.Model, ct);
        var entity = Mapper.Map<TEntity>(request.Model);

        await OnBeforeAddAsync(entity, request, ct);
        await Repo.AddAsync(entity, ct);
        await Uow.SaveChangesAsync(true, ct);
        await OnAfterAddAsync(entity, request, ct);

        return entity.Id;
    }

    // Hooks for customization without copy-paste:
    protected virtual Task OnBeforeMapAsync(TCreateDto dto, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnBeforeAddAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnAfterAddAsync(TEntity entity, TCommand request, CancellationToken ct) => Task.CompletedTask;
}