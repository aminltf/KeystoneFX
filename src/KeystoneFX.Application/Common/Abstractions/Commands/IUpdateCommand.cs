using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IUpdateCommand<TUpdateDto, TKey> : IRequest<bool>
    where TKey : IEquatable<TKey>
{
    TKey Id { get; }
    TUpdateDto Model { get; }
}