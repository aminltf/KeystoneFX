using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record DeleteCommandBase<TId>(TId Id)
    : IDeleteCommand<TId>;