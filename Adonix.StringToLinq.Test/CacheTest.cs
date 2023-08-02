using Adonix.StringToLinq.Caching;
using Xunit;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Adonix.StringToLinq.Test
{
    public class CacheTest
    {
        [Fact]
        public void TestCache()
        {

            var query = "FirstName eq \"Alex\"";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 10000; i++)
            {
                StringExpression.ToPredicate<Employee>(query);
            }
            stopwatch.Stop();

            var cache = new MemoryCache(new MemoryCacheOptions());

            Stopwatch stopwatchCache = new Stopwatch();
            stopwatchCache.Start();
            for (var i = 0; i < 10000; i++)
            {
                StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromMinutes(5));
            }
            stopwatchCache.Stop();

            Assert.True(stopwatchCache.ElapsedMilliseconds < stopwatch.ElapsedMilliseconds);
        }
    }
}
