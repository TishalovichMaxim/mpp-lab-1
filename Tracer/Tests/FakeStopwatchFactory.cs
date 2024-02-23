using Tests.ExampleClasses;

namespace Tests;

public class FakeStopwatchFactory : IStopwatchFactory
{
    public IStopwatch Create()
    {
        return new FakeStopwatch();
    }
}