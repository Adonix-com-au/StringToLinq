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
        if (node == null)
        {
            return null;
        }

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

        if (node.Token.Type == TokenType.Function)
        {
            if (node.Token.Value.ToLower() == "contains")
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Args[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Args[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("Contains", new[] {typeof(string)}), arg2);
            }

            if (node.Token.Value.ToLower() == "startswith")
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Args[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Args[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("StartsWith", new[] {typeof(string)}), arg2);
            }
        }

        var left = GenerateExpressionFromNode<T>(node.Left, param);
        var right = GenerateExpressionFromNode<T>(node.Right, param);

        if (node.Token.Type == TokenType.Operator || node.Token.Type == TokenType.Logical)
        {
            if (left.Type != right.Type && node.Right.Token.Type == TokenType.Literal)
            {
                right = ToExprConstant(node.Right.Token.Value, left);
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

        if (node.Token.Type == TokenType.Arithmetic)
        {
            if (left.Type != right.Type)
            {
                if (left.Type == typeof(int) && right.Type == typeof(double))
                {
                    left = Expression.Convert(left, typeof(double));
                }
                else if (right.Type == typeof(int) && left.Type == typeof(double))
                {
                    right = Expression.Convert(right, typeof(double));
                }
            }

            switch (node.Token.Value)
            {
                case "add":
                    return Expression.Add(left, right);
                case "sub":
                    return Expression.Subtract(left, right);
                case "mul":
                    return Expression.Multiply(left, right);
                case "div":
                    return Expression.Divide(left, right);
                case "divby":
                    return Expression.Divide(Expression.Convert(left, typeof(decimal)),
                        Expression.Convert(right, typeof(decimal)));
                case "mod":
                    return Expression.Modulo(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator {node.Token.Value}");
            }
        }

        return null;
    }

    private static Expression Contains(Expression left, Expression right)
    {
        var propertyType = left.Type;
        var containsMethod = propertyType.GetMethod("Contains", new[] {typeof(string)});
        var call = Expression.Call(left, containsMethod, right);
        return Expression.Call(left, containsMethod, right);
    }

    private static ConstantExpression ToExprConstant(string value, Expression expr)
    {
        var type = expr.Type;
        object val = value;
        if (type == typeof(int))
        {
            return Expression.Constant(Convert.ToInt32(value), typeof(int));
        }

        if (type == typeof(string))
        {
            return Expression.Constant(value, typeof(string));
        }

        if (type == typeof(int))
        {
            val = int.Parse(value);
        }
        else if (type == typeof(double))
        {
            val = double.Parse(value);
        }
        else if (type == typeof(bool))
        {
            val = bool.Parse(value);
        }

        return Expression.Constant(val, type);
    }
}