using System.Diagnostics;

namespace Tests.ExampleClasses;

public class MyStopwatch : IStopwatch
{
    private Stopwatch _stopwatch;

    public MyStopwatch(Stopwatch stopwatch)
    {
        _stopwatch = stopwatch;
    }

    public void Start()
    {
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
    }

    public long GetElapsedMilliseconds()
    {
        return _stopwatch.ElapsedMilliseconds;
    }
}