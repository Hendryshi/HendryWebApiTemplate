using System.Linq.Expressions;

namespace Common.Application.Services.Helpers
{
    public class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> Combine<T>(
            Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            // Get the parameter from the first expression
            var parameter = expr1.Parameters[0];

            // Replace the parameter in the second expression with the parameter from the first
            var visitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
            var body2 = visitor.Visit(expr2.Body);

            // Combine the bodies of the two expressions using AND
            var combinedBody = Expression.AndAlso(expr1.Body, body2);

            // Return the combined expression
            return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
        }
    }

    // Helper class to replace parameters
    public class ReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceParameterVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
