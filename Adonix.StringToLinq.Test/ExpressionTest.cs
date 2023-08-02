using Adonix.StringToLinq.Test;
using Xunit;

namespace Adonix.StringToLinq;

public class ExpressionTest
{
    [Fact]
    public void TestEqual()
    {
        TestHelper.AssertQuery("FirstName eq \"Alex\"", "x => (x.FirstName == \"Alex\")", 1);
        TestHelper.AssertQuery("\"Alex\" eq FirstName", "x => (\"Alex\" == x.FirstName)", 1);
    }

    [Fact]
    public void TestGreaterThanOrEqual()
    {
        TestHelper.AssertQuery("Salary ge 50000", "x => (x.Salary >= 50000)", 4);
    }

    [Fact]
    public void TestList()
    {
        TestHelper.AssertQuery("Salary in [50000, 60000]", "x => value(System.Double[]).Contains(x.Salary)", 2);
    }

    [Fact]
    public void TestDate()
    {
        TestHelper.AssertQuery("Birthday eq \"28/08/1999\"", "x => (x.Birthday == 28/08/1999 12:00:00 AM)", 1);
    }

    [Fact]
    public void TestNotEqual()
    {
        TestHelper.AssertQuery("FirstName ne \"Alex\"", "x => (x.FirstName != \"Alex\")", 3);
    }

    [Fact]
    public void TestSub()
    {
        TestHelper.AssertQuery("Salary sub 80000 le 0", "x => ((x.Salary - 80000) <= 0)", 3);
    }

    [Fact]
    public void TestDiv()
    {
        TestHelper.AssertQuery("Salary div 10000 eq 5", "x => ((x.Salary / 10000) == 5)", 1);
    }

    [Fact]
    public void TestOr()
    {
        TestHelper.AssertQuery("FirstName eq \"Alex\" or FirstName eq \"Amy\"", 
            "x => ((x.FirstName == \"Alex\") OrElse (x.FirstName == \"Amy\"))", 2);
    }

    [Fact]
    public void TestIn()
    {
        TestHelper.AssertQuery("FirstName in \"Alex is cool\"", "x => \"Alex is cool\".Contains(x.FirstName)", 1);
    }

    [Fact]
    public void TestHas()
    {
        TestHelper.AssertQuery("FirstName has \"A\"", "x => x.FirstName.Contains(\"A\")", 2);
    }

    [Fact]
    public void TestGroup()
    {
        TestHelper.AssertQuery("Salary gt 100000 and (FirstName eq \"Amy\" or Department eq \"CEO\")", 
            "x => ((x.Salary > 100000) AndAlso ((x.FirstName == \"Amy\") OrElse (x.Department == \"CEO\")))", 1);
    }

    [Fact]
    public void TestNestedProperty()
    {
        TestHelper.AssertQuery("Employer.Name eq \"Test Company\"",
            "x => (x.Employer.Name == \"Test Company\")", 4);
    }
}