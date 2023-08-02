namespace Adonix.StringToLinq;

internal static class Operators
{
    internal static class Comparison
    {
        internal const string Equal = "eq";

        internal const string NotEqual = "ne";

        internal const string LessThan = "lt";

        internal const string GreaterThan = "gt";

        internal const string LessThanOrEqual = "le";

        internal const string GreaterThanOrEqual = "ge";

        internal const string In = "in";

        internal const string Has = "has";

        internal static string[] collection =
            {Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual, In, Has};
    }

    internal class Logical
    {
        internal const string And = "and";

        internal const string Or = "or";

        internal const string Not = "not";

        internal static string[] collection => new[] {And, Or, Not};
    }

    internal static class Arithmetic
    {
        internal const string Add = "add";

        internal const string Sub = "sub";

        internal const string Mul = "mul";

        internal const string Div = "div";

        internal const string DivBy = "divby";

        internal const string Mod = "mod";

        internal static string[] collection = {Add, Sub, Mul, Div, DivBy, Mod};
    }

    internal static class Functions
    {
        // String Functions
        internal const string Contains = "contains";

        internal const string StartsWith = "startswith";

        internal const string EndsWith = "endswith";

        internal const string Concat = "concat";

        internal const string Length = "length";

        internal const string IndexOf = "indexof";

        internal const string SubString = "substring";


        // Date Functions

        internal const string Day = "day";

        internal static string[] collection = {Contains, StartsWith, EndsWith, Concat, Length, IndexOf, SubString, Day };
    }
}