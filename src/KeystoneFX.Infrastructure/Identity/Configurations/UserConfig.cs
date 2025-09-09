using KeystoneFX.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Concurrency
        builder.Property(x => x.RowVersion)
         .IsRowVersion()
         .IsConcurrencyToken();

        // Lengths (align with Identity)
        builder.Property(u => u.UserName).HasMaxLength(256);
        builder.Property(u => u.NormalizedUserName).HasMaxLength(256);
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256);
        builder.Property(u => u.FirstName).HasMaxLength(100);
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.NationalCode).HasMaxLength(20);

        // Indexes
        builder.HasIndex(u => u.NormalizedUserName).IsUnique();
        builder.HasIndex(u => u.NormalizedEmail);
        builder.HasIndex(u => u.NationalCode).IsUnique(false);
        builder.HasIndex(u => u.IsDeleted);

        // Global Query Filter for soft-delete
        builder.HasQueryFilter(u => !u.IsDeleted);

        // Relations
        builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.UserClaims).WithOne(uc => uc.User).HasForeignKey(uc => uc.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.UserLogins).WithOne(ul => ul.User).HasForeignKey(ul => ul.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.UserTokens).WithOne(ut => ut.User).HasForeignKey(ut => ut.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(u => u.RefreshTokens).WithOne(rt => rt.User).HasForeignKey(rt => rt.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}