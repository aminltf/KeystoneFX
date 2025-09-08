using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class UserRole : IdentityUserRole<Guid>, ITimeCreation
{
    public DateTimeOffset CreatedOnUtc { get; set; }

    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}