using System.Linq.Expressions;

namespace KeystoneFX.Shared.Kernel.Core.Querying.Spec;

public record OrderExpression<TEntity>(
        Expression<Func<TEntity, object>> KeySelector,
        bool Descending);