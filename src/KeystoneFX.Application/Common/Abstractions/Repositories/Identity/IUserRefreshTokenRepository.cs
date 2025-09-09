using KeystoneFX.Domain.Identity;

namespace KeystoneFX.Application.Common.Abstractions.Repositories.Identity;

public interface IUserRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByTokenAsync(string token, bool includeRevoked, CancellationToken ct);
    Task<UserRefreshToken?> GetActiveByTokenAsync(string token, DateTimeOffset nowUtc, CancellationToken ct);
    Task AddAsync(UserRefreshToken entity, CancellationToken ct);
    Task UpdateAsync(UserRefreshToken entity, CancellationToken ct);
}