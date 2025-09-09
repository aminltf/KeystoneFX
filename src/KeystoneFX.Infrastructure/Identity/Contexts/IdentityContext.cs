using KeystoneFX.Application.Common.Abstractions.Contexts;
using KeystoneFX.Domain.Identity;
using KeystoneFX.Infrastructure.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Contexts;

public class IdentityContext
    : IdentityDbContext<
        User, Role, Guid,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>, IIdentityContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

    public DbSet<UserRefreshToken> RefreshTokens => Set<UserRefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);

        builder.AddSoftDeleteQueryFilters();
    }
}