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
            if (int.TryParse(node.Token.Value, out var num))
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
            if (node.Token.Value.ToLower() == Operators.Functions.Contains)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("Contains", new[] {typeof(string)}), arg2);
            }

            if (node.Token.Value.ToLower() == Operators.Functions.StartsWith)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("StartsWith", new[] {typeof(string)}), arg2);
            }

            if (node.Token.Value.ToLower() == Operators.Functions.EndsWith)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("EndsWith", new[] {typeof(string)}), arg2);
            }

            if (node.Token.Value.ToLower() == Operators.Functions.Concat)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
                return Expression.Call(null, typeof(string).GetMethod("Concat", new[] {typeof(string), typeof(string)}),
                    arg1, arg2);
            }

            if (node.Token.Value.ToLower() == Operators.Functions.IndexOf)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
                return Expression.Call(arg1, typeof(string).GetMethod("IndexOf", new[] {typeof(string)}), arg2);
            }

            //if (node.Token.Value.ToLower() == Operators.Functions.SubString)
            //{
            //    var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
            //    var arg2 = GenerateExpressionFromNode<T>(node.Children[1], param);
            //    return Expression.Call(arg1, typeof(string).GetMethod("Substring", new[] {typeof(int)}), arg2);
            //}

            if (node.Token.Value.ToLower() == Operators.Functions.Length)
            {
                var arg1 = GenerateExpressionFromNode<T>(node.Children[0], param);
                return Expression.Property(arg1, "Length");
            }
        }

        var left = GenerateExpressionFromNode<T>(node.Left, param);
        var right = GenerateExpressionFromNode<T>(node.Right, param);

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
                right = ToExprConstant(node.Right.Token.Value, left);
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
        var propertyType = left.Type;
        var containsMethod = propertyType.GetMethod("Contains", new[] {typeof(string)});
        var call = Expression.Call(left, containsMethod, right);
        return Expression.Call(left, containsMethod, right);
    }

    private static ConstantExpression ToExprConstant(string value, Expression expr)
    {
        var type = expr.Type;
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
}