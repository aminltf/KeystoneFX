using KeystoneFX.Application.Common.Abstractions.Queries;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Core.Querying.Spec;
using MediatR;

namespace KeystoneFX.Application.Common.Queries.Handlers;

public abstract class GetByIdQueryHandlerBase<TQuery, TDetailDto, TEntity, TKey>
    : IRequestHandler<TQuery, TDetailDto>
    where TQuery : IGetByIdQuery<TDetailDto, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly IReadRepository<TEntity, TKey> Repo;

    protected GetByIdQueryHandlerBase(IReadRepository<TEntity, TKey> repo)
    {
        Repo = repo;
    }

    public virtual async Task<TDetailDto> Handle(TQuery request, CancellationToken ct)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var spec = BuildProjectionSpec(request)
                   ?? throw new InvalidOperationException("BuildProjectionSpec must not return null.");

        var dto = await Repo.GetAsync(spec, includeDeleted: request.IncludeDeleted, ct);

        if (dto is null)
            throw CreateNotFound(request);

        return dto;
    }

    protected virtual IProjectedSpecification<TEntity, TDetailDto>? BuildProjectionSpec(TQuery request) => null;

    // Hooks
    protected virtual Exception CreateNotFound(TQuery request)
                => new KeyNotFoundException($"{typeof(TEntity).Name} with id '{request.Id}' was not found.");
}