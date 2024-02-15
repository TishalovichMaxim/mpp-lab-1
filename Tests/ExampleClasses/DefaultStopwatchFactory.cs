using System.Diagnostics;

namespace Tests.ExampleClasses;

public class DefaultStopwatchFactory : IStopwatchFactory
{
    public IStopwatch Create()
    {
        return new MyStopwatch(new Stopwatch());
    }
}