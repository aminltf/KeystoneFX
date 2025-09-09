using KeystoneFX.Application.Common.Abstractions.Repositories.Identity;
using KeystoneFX.Domain.Identity;
using KeystoneFX.Infrastructure.Identity.Contexts;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Identity.Repositories;

public sealed class UserRefreshTokenRepository : IUserRefreshTokenRepository
{
    private readonly IdentityContext _ctx;
    public UserRefreshTokenRepository(IdentityContext ctx) => _ctx = ctx;

    public Task<UserRefreshToken?> GetByTokenAsync(string token, bool includeRevoked, CancellationToken ct)
    {
        var q = _ctx.RefreshTokens.AsQueryable();
        if (!includeRevoked) q = q.Where(x => x.RevokedOnUtc == null);
        return q.FirstOrDefaultAsync(x => x.Token == token, ct);
    }

    public Task<UserRefreshToken?> GetActiveByTokenAsync(string token, DateTimeOffset nowUtc, CancellationToken ct)
        => _ctx.RefreshTokens
               .FirstOrDefaultAsync(x => x.Token == token
                                      && x.RevokedOnUtc == null
                                      && nowUtc < x.ExpiresOnUtc, ct);

    public async Task AddAsync(UserRefreshToken entity, CancellationToken ct)
        => await _ctx.RefreshTokens.AddAsync(entity, ct);

    public Task UpdateAsync(UserRefreshToken entity, CancellationToken ct)
    {
        _ctx.RefreshTokens.Update(entity);
        return Task.CompletedTask;
    }
}