using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Core.Domain.Base;

public abstract class EntityBase<TKey> : IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; protected set; } = default!;
}