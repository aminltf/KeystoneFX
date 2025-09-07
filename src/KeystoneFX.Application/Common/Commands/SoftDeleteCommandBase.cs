using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record SoftDeleteCommandBase<TId>(TId Id)
    : ISoftDeleteCommand<TId>;