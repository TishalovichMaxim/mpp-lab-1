namespace Tracer;

public interface IStopwatch
{
    void Start();

    void Stop();

    long GetElapsedMilliseconds();
}