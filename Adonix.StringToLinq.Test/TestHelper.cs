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

        public static void AssertQueryExpection<T>(string query, string expectedMessage) where T : Exception
        {
            var ex = Assert.Throws<T>(() => { StringExpression.ToPredicate<Employee>(query); });
            Assert.Equal(expectedMessage, ex.Message);
        }
    }
}
