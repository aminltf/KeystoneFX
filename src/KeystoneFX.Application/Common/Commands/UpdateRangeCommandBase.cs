using KeystoneFX.Application.Common.Abstractions.Commands;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Application.Common.Commands;

public abstract record UpdateRangeCommandBase<TUpdateDto, TKey>(IReadOnlyCollection<TUpdateDto> Models)
    : IUpdateRangeCommand<TUpdateDto, TKey>
    where TKey : IEquatable<TKey>
    where TUpdateDto : IHasId<TKey>;