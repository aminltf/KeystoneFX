using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IRestoreCommand<TId> : IRequest<bool>
{
    TId Id { get; }
}