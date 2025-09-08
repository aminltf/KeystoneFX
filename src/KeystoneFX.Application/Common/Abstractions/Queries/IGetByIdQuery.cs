using MediatR;

namespace KeystoneFX.Application.Common.Abstractions.Queries;

public interface IGetByIdQuery<TDetailDto, TKey> : IRequest<TDetailDto> where TKey : IEquatable<TKey>
{
    TKey Id { get; }
    bool IncludeDeleted => false;
}