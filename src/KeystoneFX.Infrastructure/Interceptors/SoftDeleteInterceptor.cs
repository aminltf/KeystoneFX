using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Abstractions.Identity;
using KeystoneFX.Shared.Kernel.Abstractions.Time;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace KeystoneFX.Infrastructure.Interceptors;

public sealed class SoftDeleteInterceptor<TUserKey> : SaveChangesInterceptor
    where TUserKey : struct, IEquatable<TUserKey>
{
    private readonly ICurrentUser<TUserKey> _currentUser;
    private readonly IClock _clock;

    public SoftDeleteInterceptor(ICurrentUser<TUserKey> currentUser, IClock clock)
    {
        _currentUser = currentUser;
        _clock = clock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is DbContext ctx) ApplySoftDelete(ctx);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        if (eventData.Context is DbContext ctx) ApplySoftDelete(ctx);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void ApplySoftDelete(DbContext ctx)
    {
        var now = _clock.UtcNow;
        var isAuth = _currentUser.IsAuthenticated;
        var userId = _currentUser.UserId;

        foreach (var entry in ctx.ChangeTracker.Entries().ToArray())
        {
            if (entry.Entity is not ISoftDeletable<TUserKey> sd) continue;

            switch (entry.State)
            {
                case EntityState.Deleted:
                    // translate to soft-delete
                    entry.State = EntityState.Modified;
                    sd.IsDeleted = true;
                    if (sd.DeletedOnUtc is null) sd.DeletedOnUtc = now;
                    if (isAuth && sd.DeletedBy is null) sd.DeletedBy = userId;

                    // also touch audit if available
                    if (entry.Entity is IAuditable<TUserKey> aud)
                    {
                        aud.ModifiedOnUtc = now;
                        if (isAuth) aud.ModifiedBy = userId;
                    }
                    break;

                case EntityState.Modified:
                    var prop = entry.Property(nameof(ISoftDeletable<TUserKey>.IsDeleted));
                    if (!prop.IsModified) break;

                    var wasDeleted = (bool)prop.OriginalValue!;
                    var isDeletedNow = (bool)prop.CurrentValue!;

                    // restore
                    if (wasDeleted && !isDeletedNow)
                    {
                        sd.DeletedBy = default;
                        sd.DeletedOnUtc = null;
                        sd.DeletionReason = null;

                        if (entry.Entity is IAuditable<TUserKey> aud2)
                        {
                            aud2.ModifiedOnUtc = now;
                            if (isAuth) aud2.ModifiedBy = userId;
                        }
                    }
                    // mark delete (direct flagging)
                    else if (!wasDeleted && isDeletedNow)
                    {
                        if (sd.DeletedOnUtc is null) sd.DeletedOnUtc = now;
                        if (isAuth && sd.DeletedBy is null) sd.DeletedBy = userId;
                    }
                    break;
            }
        }
    }
}