using System.Linq.Expressions;

namespace Adonix.StringToLinq;

public static class StringExpression
{
    public static Expression<Func<T, bool>> ToPredicate<T>(string query)
    {
        var tokens = Tokenizer.Parse(query);
        var ast = new AstParser(tokens).Parse();
        return ExpressionBuilder.GenerateExpression<T>(ast);
    }
}