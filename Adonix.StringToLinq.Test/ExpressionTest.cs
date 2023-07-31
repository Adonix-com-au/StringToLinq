using Xunit;

namespace Adonix.StringToLinq.Test;

public class ExpressionTest
{
    [Fact]
    public void TestBasic()
    {
        var query = "Name eq \"Alex Mitrakis\"";
        var predicate = StringExpression.ToPredicate<Employee>(query);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();

        Assert.Equal(1, results.Count());
        Assert.Equal("Alex Mitrakis", results.FirstOrDefault().Name);
    }

    [Fact]
    public void TestGroup()
    {
        var query = "Name eq \"Alex Mitrakis\" or Name eq \"Amy Richards\"";
        var predicate = StringExpression.ToPredicate<Employee>(query);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();

        Assert.Equal(2, results.Count());
    }

    [Fact]
    public void TestGroupComplex()
    {
        var query = "Age eq 24 or (Name eq \"Amy Richards\" and Department eq \"CEO\")";
        var predicate = StringExpression.ToPredicate<Employee>(query);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();

        Assert.Equal(2, results.Count());
    }

    [Fact]
    public void TestNestedProperty()
    {
        var query = "Employer.Name eq \"Test Company\"";
        var predicate = StringExpression.ToPredicate<Employee>(query);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();

        Assert.Equal(4, results.Count());
    }
}