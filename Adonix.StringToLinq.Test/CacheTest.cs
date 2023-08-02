using Adonix.StringToLinq.Caching;
using Xunit;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Adonix.StringToLinq.Test
{
    public class CacheTest
    {
        [Fact]
        public void TestCacheFasterThanNoCache()
        {
            var query = "FirstName eq \"Alex\"";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var i = 0; i < 1000; i++)
            {
                StringExpression.ToPredicate<Employee>(query);
            }
            stopwatch.Stop();

            var cache = new MemoryCache(new MemoryCacheOptions());

            Stopwatch stopwatchCache = new Stopwatch();
            stopwatchCache.Start();
            for (var i = 0; i < 1000; i++)
            {
                StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromMinutes(5));
            }
            stopwatchCache.Stop();

            Assert.True(stopwatchCache.ElapsedMilliseconds < stopwatch.ElapsedMilliseconds);
        }

        [Fact]
        public void TestCacheUsed()
        {
            var query = "FirstName eq \"Alex\"";
            var cache = new MemoryCache(new MemoryCacheOptions());
            var value = StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromMinutes(5));
            Assert.False(value.FromCache);
            value = StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromMinutes(5));
            Assert.True(value.FromCache);
        }

        [Fact]
        public void TestCacheUsedTimeOut()
        {
            var query = "FirstName eq \"Alex\"";
            var cache = new MemoryCache(new MemoryCacheOptions());
            var value = StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromSeconds(1));
            Assert.False(value.FromCache);

            Thread.Sleep(1000);

            value = StringExpressionWithCache.ToPredicate<Employee>(query, cache, TimeSpan.FromSeconds(5));
            Assert.False(value.FromCache);
        }
    }
}
