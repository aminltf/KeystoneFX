using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record DeleteRangeCommandBase<TId>(IEnumerable<TId> Ids)
    : IDeleteRangeCommand<TId>;