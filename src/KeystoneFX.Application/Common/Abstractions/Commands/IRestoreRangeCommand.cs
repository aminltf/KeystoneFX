using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IRestoreRangeCommand<TId> : IRequest<int>
{
    IEnumerable<TId> Ids { get; }
}