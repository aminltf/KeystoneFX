using System.ComponentModel.DataAnnotations.Schema;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.AspNetCore.Identity;

namespace KeystoneFX.Domain.Identity;

public class User
    : IdentityUser<Guid>,
      IHasId<Guid>,
      IHasRowVersion,
      IAuditable<Guid>,
      ISoftDeletable<Guid>
{
    // IHasRowVersion
    public byte[]? RowVersion { get; set; }

    // IAuditable<Guid>
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }

    // ISoftDeletable<Guid>
    public bool IsDeleted { get; set; }
    public Guid? DeletedBy { get; set; }
    public DateTimeOffset? DeletedOnUtc { get; set; }
    public string? DeletionReason { get; set; }

    // Other Properties
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string NationalCode { get; set; } = null!;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<UserClaim> UserClaims { get; set; } = [];
    public ICollection<UserLogin> UserLogins { get; set; } = [];
    public ICollection<UserToken> UserTokens { get; set; } = [];
    public ICollection<UserRefreshToken> RefreshTokens { get; set; } = [];

    #region Domain helpers
    public void SoftDelete(Guid by, string? reason, DateTimeOffset nowUtc)
    {
        if (IsDeleted) return;
        IsDeleted = true;
        DeletedBy = by;
        DeletedOnUtc = nowUtc;
        DeletionReason = reason;
    }

    public void Restore(Guid by, DateTimeOffset nowUtc)
    {
        if (!IsDeleted) return;
        IsDeleted = false;
        DeletedBy = null;
        DeletedOnUtc = null;
        DeletionReason = null;
        ModifiedBy = by;
        ModifiedOnUtc = nowUtc;
    }
    #endregion
}