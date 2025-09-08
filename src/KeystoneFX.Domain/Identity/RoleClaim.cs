using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class RoleClaim : IdentityRoleClaim<Guid>, ITimeCreation
{
    public DateTimeOffset CreatedOnUtc { get; set; }

    public Role Role { get; set; } = default!;
}