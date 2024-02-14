using System.Diagnostics;

namespace Tracer;

public class StopwatchImpl : IStopwatch
{
    private readonly Stopwatch _stopwatch = new();
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