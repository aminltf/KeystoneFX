using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Core.Domain.Base;

public abstract class AuditableEntityBase<TKey, TUserKey> : EntityBase<TKey>, IAuditable<TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : struct, IEquatable<TUserKey>
{
    public TUserKey? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public TUserKey? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }
}