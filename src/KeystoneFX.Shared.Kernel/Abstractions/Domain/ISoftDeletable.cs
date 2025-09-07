namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface ISoftDeletable<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    bool IsDeleted { get; set; }
    TUserKey? DeletedBy { get; set; }
    DateTimeOffset? DeletedOnUtc { get; set; }
    string? DeletionReason { get; set; }
}