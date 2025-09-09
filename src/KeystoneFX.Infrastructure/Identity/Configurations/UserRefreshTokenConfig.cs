using KeystoneFX.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Configurations;

public class UserRefreshTokenConfig : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> b)
    {
        b.ToTable("UserRefreshTokens");

        b.Property(rt => rt.Token).IsRequired().HasMaxLength(2000);
        b.Property(rt => rt.CreatedOnUtc).HasColumnType("datetimeoffset(7)");
        b.Property(rt => rt.ExpiresOnUtc).HasColumnType("datetimeoffset(7)");
        b.Property(rt => rt.RevokedOnUtc).HasColumnType("datetimeoffset(7)");

        b.HasIndex(rt => rt.Token).IsUnique();
        b.HasIndex(rt => new { rt.UserId, rt.RevokedOnUtc, rt.ExpiresOnUtc });

        b.HasOne(rt => rt.User)
         .WithMany(u => u.RefreshTokens)
         .HasForeignKey(rt => rt.UserId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}