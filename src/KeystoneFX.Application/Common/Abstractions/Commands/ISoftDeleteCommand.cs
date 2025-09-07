using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface ISoftDeleteCommand<TId> : IRequest<bool>
{
    TId Id { get; }
}