using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record CreateCommandBase<TCreateDto, TId>(TCreateDto Model)
    : ICreateCommand<TCreateDto, TId>;