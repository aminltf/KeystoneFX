using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Abstractions.Identity;
using KeystoneFX.Shared.Kernel.Abstractions.Time;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Interceptors;

public sealed class AuditingInterceptor<TUserKey> : SaveChangesInterceptor
    where TUserKey : struct, IEquatable<TUserKey>
{
    private readonly ICurrentUser<TUserKey> _currentUser;
    private readonly IClock _clock;

    public AuditingInterceptor(ICurrentUser<TUserKey> currentUser, IClock clock)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is DbContext ctx) ApplyAuditing(ctx);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        if (eventData.Context is DbContext ctx) ApplyAuditing(ctx);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void ApplyAuditing(DbContext ctx)
    {
        var now = _clock.UtcNow;
        var isAuth = _currentUser.IsAuthenticated;
        var userId = _currentUser.UserId;

        foreach (var e in ctx.ChangeTracker.Entries().Where(x =>
                 x.State is EntityState.Added or EntityState.Modified))
        {
            if (e.Entity is IAuditable<TUserKey> aud)
            {
                if (e.State == EntityState.Added)
                {
                    if (aud.CreatedOnUtc == default) aud.CreatedOnUtc = now;
                    if (isAuth && aud.CreatedBy is null) aud.CreatedBy = userId;
                }
                else if (e.State == EntityState.Modified && HasNonConcurrencyChanges(e))
                {
                    aud.ModifiedOnUtc = now;
                    if (isAuth) aud.ModifiedBy = userId;
                }
            }
        }
    }

    private static bool HasNonConcurrencyChanges(EntityEntry entry) =>
        entry.Properties.Any(p => p.IsModified && !p.Metadata.IsConcurrencyToken);
}