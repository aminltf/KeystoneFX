using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Commands;

public interface IUpdateRangeCommand<TUpdateDto, TKey> : IRequest<int>
        where TKey : IEquatable<TKey>
        where TUpdateDto : IHasId<TKey>
{
    IReadOnlyCollection<TUpdateDto> Models { get; }
}