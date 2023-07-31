using System.Linq.Expressions;
using Adonix.StringToLinq;

internal static class ExpressionBuilder
{
    internal static Expression<Func<T, bool>> GenerateExpression<T>(Node ast)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = GenerateExpressionFromNode<T>(ast, parameter);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression GenerateExpressionFromNode<T>(Node node, ParameterExpression param)
    {
        if (node.Token.Type == TokenType.Literal)
        {
            if (double.TryParse(node.Token.Value, out var num))
            {
                return Expression.Constant(num);
            }

            return Expression.Constant(node.Token.Value);
        }

        if (node.Token.Type == TokenType.Variable)
        {
            var parts = node.Token.Value.Split('.');
            Expression propertyAccess = param;
            foreach (var part in parts)
            {
                propertyAccess = Expression.Property(propertyAccess, part);
            }

            return propertyAccess;
        }

        var left = GenerateExpressionFromNode<T>(node.Left, param);
        var right = GenerateExpressionFromNode<T>(node.Right, param);

        if (node.Token.Type == TokenType.Operator)
        {
            var parts = node.Left.Token.Value.Split('.');
            Expression propertyAccess = param;

            foreach (var part in parts)
            {
                propertyAccess = Expression.Property(propertyAccess, part);
            }

            if (right is ConstantExpression constant)
            {
                right = ToExprConstant(propertyAccess.Type, constant.Value);
            }
        }

        switch (node.Token.Value)
        {
            case "and":
                return Expression.AndAlso(left, right);
            case "or":
                return Expression.OrElse(left, right);
            case "eq":
                return Expression.Equal(left, right);
            case "ne":
                return Expression.NotEqual(left, right);
            case "lt":
                return Expression.LessThan(left, right);
            case "gt":
                return Expression.GreaterThan(left, right);
            case "le":
                return Expression.LessThanOrEqual(left, right);
            case "ge":
                return Expression.GreaterThanOrEqual(left, right);
            case "in":
                return Contains(left, right);
            default:
                throw new NotSupportedException($"Operator '{node.Token.Value}' is not supported.");
        }
    }

    private static Expression Contains(Expression left, Expression right)
    {
        var propertyType = left.Type;
        var containsMethod = propertyType.GetMethod("Contains", new[] {typeof(string)});
        var call = Expression.Call(left, containsMethod, right);
        return Expression.Call(left, containsMethod, right);
    }

    private static Expression ToExprConstant(Type prop, object value)
    {
        object val;
        if (prop == typeof(Guid))
        {
            val = Guid.Parse((string) value);
        }
        else
        {
            val = Convert.ChangeType(value, prop);
        }

        return Expression.Constant(val);
    }
}