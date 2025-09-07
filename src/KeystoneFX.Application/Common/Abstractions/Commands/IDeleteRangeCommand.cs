using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IDeleteRangeCommand<TId> : IRequest<int>
{
    IEnumerable<TId> Ids { get; }
}