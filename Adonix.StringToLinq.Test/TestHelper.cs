using Xunit;

namespace Adonix.StringToLinq.Test
{
    public static class TestHelper
    {
        public static void AssertQuery(string query, string expectedPredicate, int expectedCount)
        {
            var predicate = StringExpression.ToPredicate<Employee>(query);
            var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
            Assert.Equal(expectedPredicate, predicate.ToString());
            Assert.Equal(expectedCount, results.Count());
        }
    }
}
