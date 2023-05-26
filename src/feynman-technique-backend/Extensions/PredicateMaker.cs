using System.Linq.Expressions;

namespace FeynmanTechniqueBackend.Extensions
{
    public static class PredicateMaker
    {
        public static Expression<Func<E, bool>> And<E>(this Expression<Func<E, bool>> expr1, Expression<Func<E, bool>> expr2)
        {
            InvocationExpression invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<E, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
