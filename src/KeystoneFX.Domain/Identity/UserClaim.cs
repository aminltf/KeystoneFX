using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class UserClaim : IdentityUserClaim<Guid>
{
    public User User { get; set; } = default!;
}