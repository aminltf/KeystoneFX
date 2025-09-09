using System.Reflection;
using KeystoneFX.Shared.Kernel.Abstractions.Data;
using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace KeystoneFX.Infrastructure.Persistence.Repositories;

public class EfWriteRepository<TEntity, TKey> : IWriteRepository<TEntity, TKey>
    where TEntity : class, IHasId<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly DbContext _db;
    private readonly DbSet<TEntity> _set;

    public EfWriteRepository(DbContext context)
    {
        _db = context;
        _set = _db.Set<TEntity>();
    }

    public Task<TEntity?> LoadByIdAsync(
            TKey id,
            bool includeDeleted = false,
            CancellationToken ct = default)
    {
        var q = includeDeleted ? _set.IgnoreQueryFilters() : _set;
        return q.AsTracking().FirstOrDefaultAsync(e => e.Id!.Equals(id), ct);
    }

    public async Task<IReadOnlyList<TEntity>> LoadByIdsAsync(
        IEnumerable<TKey> ids,
        bool includeDeleted = false,
        CancellationToken ct = default)
    {
        if (ids is null) throw new ArgumentNullException(nameof(ids));
        var idSet = ids.Distinct().ToArray();
        if (idSet.Length == 0) return Array.Empty<TEntity>();

        var q = includeDeleted ? _set.IgnoreQueryFilters() : _set;
        return await q.AsTracking()
                      .Where(e => idSet.Contains(e.Id))
                      .ToListAsync(ct);
    }

    public Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        return _set.AddAsync(entity, ct).AsTask();
    }

    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        if (entities is null) throw new ArgumentNullException(nameof(entities));
        return _set.AddRangeAsync(entities, ct);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        var entry = _db.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _set.Attach(entity);
        }
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default)
    {
        if (entities is null) throw new ArgumentNullException(nameof(entities));
        foreach (var e in entities)
        {
            var entry = _db.Entry(e);
            if (entry.State == EntityState.Detached)
                _set.Attach(e);
        }
        return Task.CompletedTask;
    }

    public Task RemoveAsync(TKey id, CancellationToken ct = default)
    {
        var stub = CreateStub(id);
        _db.Entry(stub).State = EntityState.Deleted;
        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        if (ids is null) throw new ArgumentNullException(nameof(ids));
        foreach (var id in ids.Distinct())
        {
            var stub = CreateStub(id);
            _db.Entry(stub).State = EntityState.Deleted;
        }
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await LoadByIdAsync(id, includeDeleted: true, ct)
                     ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with id '{id}' not found.");

        if (!TryMarkSoftDeleted(entity))
            throw new NotSupportedException($"{typeof(TEntity).Name} does not support soft delete.");
    }

    public async Task SoftDeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        if (ids is null) throw new ArgumentNullException(nameof(ids));
        var list = await LoadByIdsAsync(ids, includeDeleted: true, ct);
        if (list.Count == 0) return;

        foreach (var e in list)
        {
            if (!TryMarkSoftDeleted(e))
                throw new NotSupportedException($"{typeof(TEntity).Name} does not support soft delete.");
        }
    }

    public async Task RestoreAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await LoadByIdAsync(id, includeDeleted: true, ct)
                     ?? throw new KeyNotFoundException($"{typeof(TEntity).Name} with id '{id}' not found.");

        if (!TryMarkRestored(entity))
            throw new NotSupportedException($"{typeof(TEntity).Name} does not support restore.");
    }

    public async Task RestoreRangeAsync(IEnumerable<TKey> ids, CancellationToken ct = default)
    {
        if (ids is null) throw new ArgumentNullException(nameof(ids));
        var list = await LoadByIdsAsync(ids, includeDeleted: true, ct);
        if (list.Count == 0) return;

        foreach (var e in list)
        {
            if (!TryMarkRestored(e))
                throw new NotSupportedException($"{typeof(TEntity).Name} does not support restore.");
        }
    }

    public void Attach(TEntity entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (_db.Entry(entity).State == EntityState.Detached)
            _set.Attach(entity);
    }

    public void Detach(TEntity entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        _db.Entry(entity).State = EntityState.Detached;
    }

    // Helpers
    private TEntity CreateStub(TKey id)
    {
        var entity = Activator.CreateInstance<TEntity>()
                     ?? throw new InvalidOperationException($"Cannot create instance of {typeof(TEntity).Name}.");

        EntityEntry entry = _db.Entry(entity);

        var idProp = entry.Metadata.FindProperty("Id")
                  ?? entry.Metadata.FindPrimaryKey()?.Properties.SingleOrDefault();

        if (idProp is null)
            throw new InvalidOperationException($"Key property not found for {typeof(TEntity).Name}.");

        entry.Property(idProp.Name).CurrentValue = id!;
        return entity;
    }

    private static bool TryMarkSoftDeleted(TEntity entity, object? deletedBy = null, string? reason = null)
    {
        if (!ImplementsISoftDeletable(entity)) return false;

        var ok = true;
        ok &= TrySetProperty(entity, "IsDeleted", true);
        ok &= TrySetProperty(entity, "DeletedOnUtc", DateTimeOffset.UtcNow);

        if (deletedBy is not null) TrySetProperty(entity, "DeletedBy", deletedBy);
        if (reason is not null) TrySetProperty(entity, "DeletionReason", reason);

        return ok;
    }

    private static bool TryMarkRestored(TEntity entity)
    {
        if (!ImplementsISoftDeletable(entity)) return false;

        var ok = true;
        ok &= TrySetProperty(entity, "IsDeleted", false);
        TrySetProperty(entity, "DeletedOnUtc", null);
        TrySetProperty(entity, "DeletedBy", null);
        TrySetProperty(entity, "DeletionReason", null);

        return ok;
    }

    private static bool ImplementsISoftDeletable(object entity)
    {
        return entity.GetType()
                     .GetInterfaces()
                     .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Name is "ISoftDeletable`1"
                                                       && i.GetGenericTypeDefinition().FullName!.Contains("ISoftDeletable"));
    }

    private static bool TrySetProperty(object target, string name, object? value)
    {
        var prop = target.GetType().GetProperty(
            name,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (prop is null) return false;

        var setter = prop.GetSetMethod(nonPublic: true);
        if (setter is null) return false; 

        if (value is null)
        {
            setter.Invoke(target, [null]);
            return true;
        }

        var destType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        object? toAssign = value;

        if (!destType.IsInstanceOfType(value))
        {
            try
            {
                toAssign = Convert.ChangeType(value, destType);
            }
            catch
            {
                return false;
            }
        }

        setter.Invoke(target, [toAssign]);
        return true;
    }
}