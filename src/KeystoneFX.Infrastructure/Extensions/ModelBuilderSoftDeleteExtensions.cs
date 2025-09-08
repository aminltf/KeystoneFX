using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KeystoneFX.Infrastructure.Extensions;

public static class ModelBuilderSoftDeleteExtensions
{
    public static void AddSoftDeleteQueryFilters(this ModelBuilder builder)
    {
        foreach (var et in builder.Model.GetEntityTypes())
        {
            var clr = et.ClrType;
            var implements = clr.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISoftDeletable<>));
            if (implements is null) continue;

            var param = Expression.Parameter(clr, "e");
            var isDeletedProp = Expression.Property(param, nameof(ISoftDeletable<int>.IsDeleted)); // name-based
            var filter = Expression.Lambda(Expression.Equal(isDeletedProp, Expression.Constant(false)), param);

            et.SetQueryFilter(filter);
            // Optional: shadow index on IsDeleted
            builder.Entity(clr).HasIndex(nameof(ISoftDeletable<int>.IsDeleted));
        }
    }
}