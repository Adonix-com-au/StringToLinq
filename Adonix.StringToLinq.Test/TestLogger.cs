
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

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