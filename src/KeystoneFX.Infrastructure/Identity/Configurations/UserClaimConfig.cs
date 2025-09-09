using KeystoneFX.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Configurations;

public class UserClaimConfig : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable("UserClaims");
        builder.HasKey(uc => uc.Id);
    }
}