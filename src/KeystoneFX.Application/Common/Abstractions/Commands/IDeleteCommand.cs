using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IDeleteCommand<TId> : IRequest<Unit>
{
    TId Id { get; }
}