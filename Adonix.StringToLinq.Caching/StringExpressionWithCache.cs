using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Caching.Memory;

namespace Adonix.StringToLinq.Caching
{
    public static class StringExpressionWithCache
    {
        public static Func<T, bool> ToPredicate<T>(string query, MemoryCache cache, TimeSpan expiryFromNow)
        {
            var key = (typeof(T), query);
            if (cache.TryGetValue(key, out Func<T, bool> function))
            {
                return function;
            }
            else
            {
                var predicate = StringExpression.ToPredicate<T>(query).Compile();
                cache.Set(key, predicate, new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = expiryFromNow,
                    Size = 1
                });
                return predicate;
            }
        }
    }
}