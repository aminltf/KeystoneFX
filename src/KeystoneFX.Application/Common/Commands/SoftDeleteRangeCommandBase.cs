using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record SoftDeleteRangeCommandBase<TId>(IEnumerable<TId> Ids)
    : ISoftDeleteRangeCommand<TId>;