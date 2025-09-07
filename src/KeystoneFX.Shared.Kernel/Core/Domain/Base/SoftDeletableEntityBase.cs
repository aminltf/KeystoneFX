using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Core.Domain.Base;

public abstract class SoftDeletableEntityBase<TKey, TUserKey> : EntityBase<TKey>, ISoftDeletable<TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : IEquatable<TUserKey>
{
    public bool IsDeleted { get; set; }
    public TUserKey? DeletedBy { get; set; }
    public DateTimeOffset? DeletedOnUtc { get; set; }
    public string? DeletionReason { get; set; }
}