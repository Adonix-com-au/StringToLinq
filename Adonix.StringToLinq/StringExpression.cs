using System.Linq.Expressions;
using Microsoft.Extensions.Logging;

namespace Adonix.StringToLinq;

public static class StringExpression
{
    public static Expression<Func<T, bool>> ToPredicate<T>(string query)
    {
        return ExpressionBuilder.GenerateExpression<T>(CreateAST(query).Parse());
    }

    private static AstParser CreateAST(string query)
    {
        return new AstParser(Tokenizer.Parse(query));
    }

#if DEBUG
    public static Expression<Func<T, bool>> ToPredicate<T>(string query, ILogger logger)
    {
        if (logger != null)
        {
            //Grr i know we do this twice... I don't want to expose anything besides ToPredicate
            var debugAst = CreateAST(query);
            debugAst.Parse();
            debugAst.Print(logger);
        }

        var predicate = ToPredicate<T>(query);

        logger.LogInformation(predicate.ToString());

        return predicate;
    }
#endif
}