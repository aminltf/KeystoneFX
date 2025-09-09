using KeystoneFX.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Configurations;

public class RoleClaimConfig : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaims");
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.CreatedOnUtc).HasColumnType("datetimeoffset(7)");
    }
}