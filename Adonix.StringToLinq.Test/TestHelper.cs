using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Adonix.StringToLinq.Test
{
    public static class TestHelper
    {

        public static void AssertQuery(string query, string expectedPredicate, int expectedCount, TestLogger logger)
        {
            var predicate = StringExpression.ToPredicate<Employee>(query, logger);
            var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
            Assert.True(logger.Contains(expectedPredicate));
            Assert.Equal(expectedCount, results.Count());
        }
    }
}
