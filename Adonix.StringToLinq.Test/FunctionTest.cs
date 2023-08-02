using Adonix.StringToLinq.Test;
using Xunit;
using Xunit.Abstractions;

namespace Adonix.StringToLinq;

public class FunctionTest
{
    [Fact]
    public void TestContains()
    {
        TestHelper.AssertQuery("contains(FirstName, \"Alex\")", "x => x.FirstName.Contains(\"Alex\")", 1);
        TestHelper.AssertQueryExpection<ArgumentException>("contains(FirstName, FirstName, FirstName)", "Contains requires 2 parameters");
    }

    [Fact]
    public void TestStartsWith()
    {
        TestHelper.AssertQuery("startswith(FirstName, \"Alex\")", "x => x.FirstName.StartsWith(\"Alex\")", 1);
    }

    [Fact]
    public void TestIndexOf()
    {
        TestHelper.AssertQuery("indexof(FirstName, \"A\") eq 0", "x => (x.FirstName.IndexOf(\"A\") == 0)", 2);
    }

    [Fact]
    public void TestEndsWith()
    {
        TestHelper.AssertQuery("endswith(LastName, \"is\")", "x => x.LastName.EndsWith(\"is\")", 1);
    }

    [Fact]
    public void TestLength()
    {
        TestHelper.AssertQuery("length(FirstName) gt 0", "x => (x.FirstName.Length > 0)", 4);
    }

    [Fact]
    public void TestConcat()
    {
        TestHelper.AssertQuery("concat(\"A\",\"A\") eq \"AA\"", "x => (Concat(\"A\", \"A\") == \"AA\")", 4);
    }

    [Fact]
    public void TestDay()
    {
        TestHelper.AssertQuery("day(Birthday) eq 28", "x => (x.Birthday.Day == 28)", 1);
    }
}
