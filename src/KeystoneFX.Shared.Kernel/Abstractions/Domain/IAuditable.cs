namespace KeystoneFX.Shared.Kernel.Abstractions.Domain;

public interface IAuditable<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    TUserKey? CreatedBy { get; set; }
    DateTimeOffset CreatedOnUtc { get; set; }
    TUserKey? ModifiedBy { get; set; }
    DateTimeOffset? ModifiedOnUtc { get; set; }
}