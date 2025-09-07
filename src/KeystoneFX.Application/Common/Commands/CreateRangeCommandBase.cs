using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record CreateRangeCommandBase<TCreateDto, TKey>(IReadOnlyCollection<TCreateDto> Models)
        : ICreateRangeCommand<TCreateDto, TKey> where TKey : IEquatable<TKey>;