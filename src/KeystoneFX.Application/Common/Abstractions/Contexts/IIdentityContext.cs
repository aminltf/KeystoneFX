using KeystoneFX.Domain.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Application.Common.Abstractions.Contexts;

public interface IIdentityContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    DatabaseFacade Database { get; }
}