using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Core.Domain.Base;

public abstract class ConcurrencyEntityBase<TKey> : EntityBase<TKey>, IHasRowVersion
        where TKey : IEquatable<TKey>
{
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}