namespace KeystoneFX.Shared.Kernel.Abstractions.Identity;

public interface ICurrentUser<TUserKey> where TUserKey : IEquatable<TUserKey>
{
    bool IsAuthenticated { get; }
    TUserKey? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
}