using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class UserLogin : IdentityUserLogin<Guid>, ITimeCreation
{
    public DateTimeOffset CreatedOnUtc { get; set; }

    public User User { get; set; } = default!;
}