using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Adonix.StringToLinq;

public static class StringExpression
{
    public static Expression<Func<T, bool>> ToPredicate<T>(string query)
    {
        return ExpressionBuilder.GenerateExpression<T>(new AstParser(Tokenizer.Parse(query)).Parse());
    }


#if DEBUG
    public static Expression<Func<T, bool>> ToPredicate<T>(string query, ILogger logger)
    {
        var ast = new AstParser(Tokenizer.Parse(query));
        var root = ast.Parse();
        ast.Print(logger);
        var predicate = ExpressionBuilder.GenerateExpression<T>(root);
        logger.LogInformation(predicate.ToString());
        return predicate;
    }
#endif
}