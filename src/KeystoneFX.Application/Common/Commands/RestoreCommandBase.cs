using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record RestoreCommandBase<TId>(TId Id)
    : IRestoreCommand<TId>;