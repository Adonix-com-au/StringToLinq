using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Adonix.StringToLinq.Test;

public class ExpressionTest
{
    private readonly TestLogger logger;

    public ExpressionTest(ITestOutputHelper output)
    {
        logger = new TestLogger(output);
    }

    [Fact]
    public void TestEqual()
    {
        var query = "Name eq \"Alex Mitrakis\"";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => (x.Name == \"Alex Mitrakis\")"));
        Assert.Equal(1, results.Count());
        Assert.Equal("Alex Mitrakis", results.FirstOrDefault().Name);
    }

    [Fact]
    public void TestFunctionContains()
    {
        var query = "contains(Name, \"Alex\")";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.Equal(1, results.Count());
        Assert.Equal("Alex Mitrakis", results.FirstOrDefault().Name);
    }

    [Fact]
    public void TestFunctionStartsWith()
    {
        var query = "startswith(Name, \"Alex\")";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.Equal(1, results.Count());
        Assert.Equal("Alex Mitrakis", results.FirstOrDefault().Name);
    }

    [Fact]
    public void TestNotEqual()
    {
        var query = "Name ne \"Alex Mitrakis\"";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => (x.Name != \"Alex Mitrakis\")"));
        Assert.Equal(3, results.Count());
    }

    [Fact]
    public void TestSub()
    {
        var query = "Age sub 20 lt 20";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => ((Convert(x.Age, Double) - 20) < 20)"));
        Assert.Equal(4, results.Count());
        Assert.Equal(25, results.FirstOrDefault().Age);
    }

    [Fact]
    public void TestDiv()
    {
        var query = "Age div 12 eq 2";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => ((Convert(x.Age, Double) / 12) == 2)"));
        Assert.Equal(1, results.Count());
        Assert.Equal(24, results.FirstOrDefault().Age);
    }

    [Fact]
    public void TestMulti()
    {
        var query = "Name eq \"Alex Mitrakis\" or Name eq \"Amy Richards\"";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => ((x.Name == \"Alex Mitrakis\") OrElse (x.Name == \"Amy Richards\"))"));
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public void TestIn()
    {
        var query = "Name in \"Alex\"";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => x.Name.Contains(\"Alex\")"));
        Assert.Equal(1, results.Count());
    }

    [Fact]
    public void TestGroup()
    {
        var query = "Age eq 24 or (Name eq \"Amy Richards\" and Department eq \"CEO\")";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains(
            "x => ((x.Age == 24) OrElse ((x.Name == \"Amy Richards\") AndAlso (x.Department == \"CEO\")))"));
        Assert.Equal(2, results.Count());
    }

    [Fact]
    public void TestNestedProperty()
    {
        var query = "Employer.Name eq \"Test Company\"";
        var predicate = StringExpression.ToPredicate<Employee>(query, logger);
        var results = new EmployeeData().GetEmployees().Where(predicate).AsEnumerable();
        Assert.True(logger.Contains("x => (x.Employer.Name == \"Test Company\")"));
        Assert.Equal(4, results.Count());
    }
}

public class TestLogger : ILogger
{
    private readonly List<string> logs;
    private readonly ITestOutputHelper output;

    public TestLogger(ITestOutputHelper output)
    {
        this.output = output;
        logs = new List<string>();
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var value = state.ToString();
        logs.Add(value);
        output.WriteLine(value);
    }

    public bool Contains(string value)
    {
        return logs.Any(x => x == value);
    }
}