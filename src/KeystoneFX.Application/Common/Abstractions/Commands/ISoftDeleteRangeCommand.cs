using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface ISoftDeleteRangeCommand<TId> : IRequest<int>
{
    IEnumerable<TId> Ids { get; }
}