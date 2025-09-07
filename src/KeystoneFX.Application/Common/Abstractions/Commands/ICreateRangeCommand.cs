using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface ICreateRangeCommand<TCreateDto, TKey> : IRequest<IReadOnlyCollection<TKey>>
        where TKey : IEquatable<TKey>
{
    IReadOnlyCollection<TCreateDto> Models { get; }
}