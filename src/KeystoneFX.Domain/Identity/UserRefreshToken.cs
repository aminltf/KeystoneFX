using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Core.Domain.Base;

namespace KeystoneFX.Domain.Identity;

public class UserRefreshToken : EntityBase<Guid>, ITimeCreation
{
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

    public DateTimeOffset CreatedOnUtc { get; set; }
    public string Token { get; set; } = default!;
    public DateTimeOffset ExpiresOnUtc { get; set; }

    public string? CreatedByIp { get; set; }
    public DateTimeOffset? RevokedOnUtc { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    public bool IsActive => RevokedOnUtc == null && !IsExpired;
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOnUtc;

    public void Revoke(string ip, DateTimeOffset nowUtc, string? replacedBy = null)
    {
        RevokedOnUtc = nowUtc;
        RevokedByIp = ip;
        ReplacedByToken = replacedBy;
    }
}