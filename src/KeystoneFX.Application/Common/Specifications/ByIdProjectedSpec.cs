using KeystoneFX.Shared.Kernel.Abstractions.Domain;
using KeystoneFX.Shared.Kernel.Core.Querying.Spec;
using System.Linq.Expressions;

namespace KeystoneFX.Application.Common.Specifications;

public sealed class ByIdProjectedSpec<TEntity, TKey, TDto>
        : ProjectedSpecification<TEntity, TDto>
        where TEntity : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
{
    public ByIdProjectedSpec(
        TKey id,
        Expression<Func<TEntity, TDto>> selector)
    {
        Where(e => EqualityComparer<TKey>.Default.Equals(e.Id, id));
        Select(selector);
        AsReadOnly();
    }
}