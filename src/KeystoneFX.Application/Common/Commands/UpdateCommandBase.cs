using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record UpdateCommandBase<TKey, TUpdateDto>(TKey Id, TUpdateDto Model)
    : IUpdateCommand<TUpdateDto, TKey> where TKey : IEquatable<TKey>;