using KeystoneFX.Application.Common.Abstractions.Queries;

namespace KeystoneFX.Application.Common.Queries;

public abstract record GetByIdQueryBase<TDetailDto, TKey>(TKey Id, bool IncludeDeleted = false)
    : IGetByIdQuery<TDetailDto, TKey>
    where TKey : IEquatable<TKey>;