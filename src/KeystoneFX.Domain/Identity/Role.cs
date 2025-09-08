using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class Role
    : IdentityRole<Guid>,
      IHasId<Guid>,
      IHasRowVersion,
      IAuditable<Guid>
{
    // IHasRowVersion
    public byte[]? RowVersion { get; set; }

    // IAuditable<Guid>
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }

    // Other Properties
    public string DisplayName { get; set; } = null!;

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RoleClaim> RoleClaims { get; set; } = [];
}