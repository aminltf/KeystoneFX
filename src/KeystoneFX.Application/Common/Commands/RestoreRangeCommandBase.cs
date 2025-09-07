using KeystoneFX.Application.Common.Abstractions.Commands;

namespace KeystoneFX.Application.Common.Commands;

public abstract record RestoreRangeCommandBase<TId>(IEnumerable<TId> Ids)
    : IRestoreRangeCommand<TId>;