using System.Data;
using System.Linq.Expressions;
using System.Xml.Linq;
using Adonix.StringToLinq;

internal static class ExpressionBuilder
{
    internal static Expression<Func<T, bool>> GenerateExpression<T>(Node ast)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = GenerateExpressionFromNode<T>(ast, parameter);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private static Expression GenerateExpressionFromNode<T>(Node node, ParameterExpression param, Type leftType = null)
    {
        if (node == null)
        {
            return null;
        }

        if (node.Token.Type == TokenType.Literal)
        {
            if (leftType != null)
            {
                return ToExprConstant(node.Token.Value, leftType);
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

        if (node.Token.Type == TokenType.Collection)
        {
            var collection = node.Token.Value.Split(',');

            if (leftType != null)
            {
                if (leftType == typeof(int))
                {
                    return Expression.Constant(Array.ConvertAll(collection, s => int.Parse(s)));
                }

                if (leftType == typeof(double))
                {
                    return Expression.Constant(Array.ConvertAll(collection, s => double.Parse(s)));
                }
            }

            return Expression.Constant(collection);
        }

        if (node.Token.Type == TokenType.Function)
        {
            switch (node.Token.Value.ToLower())
            {
                case Operators.Functions.Contains:
                    return Contains<T>(node, param);
                case Operators.Functions.StartsWith:
                    return StartsWith<T>(node, param);
                case Operators.Functions.EndsWith:
                    return EndsWith<T>(node, param);
                case Operators.Functions.Concat:
                    return Concat<T>(node, param);
                case Operators.Functions.IndexOf:
                    return IndexOf<T>(node, param);
                //case Operators.Functions.SubString:
                //    return SubString<T>(node, param);
                case Operators.Functions.Length:
                    return Length<T>(node, param);
                case Operators.Functions.Day:
                    return Day<T>(node, param);
            }
        }

        var left = GenerateExpressionFromNode<T>(node.Left, param);
        var right = GenerateExpressionFromNode<T>(node.Right, param, left.Type);

        if (node.Token.Type == TokenType.Operator)
        {
            switch (node.Token.Value)
            {
                case Operators.Logical.And:
                    return Expression.AndAlso(left, right);
                case Operators.Logical.Or:
                    return Expression.OrElse(left, right);
            }
        }

        if (node.Token.Type == TokenType.Operator || node.Token.Type == TokenType.Logical)
        {
            if (left.Type != right.Type && node.Right.Token.Type == TokenType.Literal)
            {
                right = ToExprConstant(node.Right.Token.Value, left.Type);
            }

            switch (node.Token.Value)
            {
                case Operators.Logical.And:
                    return Expression.AndAlso(left, right);
                case Operators.Logical.Or:
                    return Expression.OrElse(left, right);
            }

            switch (node.Token.Value)
            {
                case Operators.Comparison.Equal:
                    return Expression.Equal(left, right);
                case Operators.Comparison.NotEqual:
                    return Expression.NotEqual(left, right);
                case Operators.Comparison.LessThan:
                    return Expression.LessThan(left, right);
                case Operators.Comparison.GreaterThan:
                    return Expression.GreaterThan(left, right);
                case Operators.Comparison.LessThanOrEqual:
                    return Expression.LessThanOrEqual(left, right);
                case Operators.Comparison.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(left, right);
                case Operators.Comparison.Has:
                    return Contains(left, right);
                case Operators.Comparison.In:
                    return Contains(right, left);
                default:
                    throw new NotSupportedException($"Operator '{node.Token.Value}' is not supported.");
            }
        }

        if (node.Token.Type == TokenType.Arithmetic)
        {
            if (left.Type != typeof(double))
            {
                left = Expression.Convert(left, typeof(double));
            }

            if (right.Type != typeof(double))
            {
                right = Expression.Convert(right, typeof(double));
            }

            switch (node.Token.Value)
            {
                case Operators.Arithmetic.Add:
                    return Expression.Add(left, right);
                case Operators.Arithmetic.Sub:
                    return Expression.Subtract(left, right);
                case Operators.Arithmetic.Mul:
                    return Expression.Multiply(left, right);
                case Operators.Arithmetic.Div:
                    return Expression.Divide(left, right);
                case Operators.Arithmetic.DivBy:
                    //TODO: This seems wrong need to fix http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#_Toc31358950
                    return Expression.Divide(left, right);
                case Operators.Arithmetic.Mod:
                    return Expression.Modulo(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator {node.Token.Value}");
            }
        }

        return null;
    }

    private static Expression Contains(Expression left, Expression right)
    {
        Expression contains = null;
        // If left is a string, use the string's Contains method.
        if (left.Type == typeof(string))
        {
            // If right is not a string, convert it to string.
            if (right.Type != typeof(string))
            {
                right = Expression.Call(right, right.Type.GetMethod("ToString", Type.EmptyTypes));
            }

            var propertyType = left.Type;
            var containsMethod = propertyType.GetMethod("Contains", new[] {typeof(string)});
            return Expression.Call(left, containsMethod, right);
        }

        // If left is a collection, use the Enumerable's Contains method.
        else if (left.Type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            Type elementType = left.Type.GetElementType() ?? left.Type.GetGenericArguments().First();
            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Count() == 2)
                .MakeGenericMethod(elementType);

            return Expression.Call(null, containsMethod, left, right);
        }

        return null;
    }

    private static ConstantExpression ToExprConstant(string value, Type type)
    {
        if (type == typeof(int))
        {
            return Expression.Constant(Convert.ToInt32(value), type);
        }

        if (type == typeof(string))
        {
            return Expression.Constant(value, type);
        }

        if (type == typeof(Guid))
        {
            return Expression.Constant(Guid.Parse(value), type);
        }

        if (type == typeof(int))
        {
            return Expression.Constant(int.Parse(value), type);
        }

        if (type == typeof(DateTime))
        {
            return Expression.Constant(DateTime.Parse(value), type);
        }

        if (type == typeof(double))
        {
            return Expression.Constant(double.Parse(value), type);
        }

        if (type == typeof(bool))
        {
            return Expression.Constant(bool.Parse(value), type);
        }

        return null;
    }

    //Functions

    internal static MemberExpression Length<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 1)
        {
            throw new ArgumentException("");
        }

        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        return Expression.Property(arg1, "Length");
    }

    internal static MethodCallExpression Substring<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }

        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(arg1, typeof(string).GetMethod("Substring", new[] { typeof(int) }), arg2);
    }

    internal static MemberExpression Day<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 1)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        if (arg1 is ConstantExpression constant)
        {
            if (constant.Value is string value)
            {
                arg1 = ToExprConstant(value, typeof(DateTime));
            }
        }
        return Expression.Property(arg1, "Day");
    }

    internal static Expression Contains<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(arg1, typeof(string).GetMethod("Contains", new[] { typeof(string) }), arg2);
    }

    internal static Expression StartsWith<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(arg1, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), arg2);
    }

    internal static MethodCallExpression EndsWith<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(arg1, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), arg2);
    }

    internal static MethodCallExpression Concat<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(null, typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) }), arg1, arg2);
    }

    internal static MethodCallExpression IndexOf<T>(Node node, ParameterExpression param)
    {
        if (node.Children.Length != 2)
        {
            throw new ArgumentException("");
        }
        var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
        var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
        return Expression.Call(arg1, typeof(string).GetMethod("IndexOf", new[] { typeof(string) }), arg2);
    }
}