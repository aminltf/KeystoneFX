using KeystoneFX.Shared.Kernel.Abstractions.Domain;

namespace KeystoneFX.Shared.Kernel.Core.Domain.Base;

public abstract class FullyTrackedEntityBase<TKey, TUserKey>
    :   ConcurrencyEntityBase<TKey>,
        IAuditable<TUserKey>,
        ISoftDeletable<TUserKey>
        where TKey : IEquatable<TKey>
        where TUserKey : struct, IEquatable<TUserKey>
{
    // Auditing
    public TUserKey? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public TUserKey? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }

    // Soft-delete
    public bool IsDeleted { get; set; }
    public TUserKey? DeletedBy { get; set; }
    public DateTimeOffset? DeletedOnUtc { get; set; }
    public string? DeletionReason { get; set; }
}