namespace Tests.ExampleClasses;

public class FakeStopwatch : IStopwatch
{
    public void Start()
    {
    }

    public void Stop()
    {
    }

    public long GetElapsedMilliseconds()
    {
        return 0L;
    }
}