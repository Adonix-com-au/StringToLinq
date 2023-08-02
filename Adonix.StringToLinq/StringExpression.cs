using System.Linq.Expressions;

namespace Adonix.StringToLinq;

public static class StringExpression
{
    public static Expression<Func<T, bool>> ToPredicate<T>(string query)
    {
        return ExpressionBuilder.GenerateExpression<T>(new AstParser(Tokenizer.Parse(query)).Parse());
    }
}