using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface ICreateCommand<TCreateDto, TId> : IRequest<TId>
{
    TCreateDto Model { get; }
}