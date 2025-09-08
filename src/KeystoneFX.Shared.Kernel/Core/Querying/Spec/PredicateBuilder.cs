using System.Linq.Expressions;

namespace KeystoneFX.Shared.Kernel.Core.Querying.Spec;

internal static class PredicateBuilder
{
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
        => Compose(left, right, Expression.AndAlso);

    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
        => Compose(left, right, Expression.OrElse);

    private static Expression<Func<T, bool>> Compose<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var param = Expression.Parameter(typeof(T), "x");

        var left = new ReplaceParameterVisitor(first.Parameters[0], param).Visit(first.Body)!;
        var right = new ReplaceParameterVisitor(second.Parameters[0], param).Visit(second.Body)!;

        return Expression.Lambda<Func<T, bool>>(merge(left, right), param);
    }

    private sealed class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;
        public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node)
            => node == _from ? _to : base.VisitParameter(node);
    }
}