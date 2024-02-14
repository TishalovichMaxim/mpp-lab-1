namespace Tracer;

public class StopwatchFactory : IStopwatchFactory
{
    public IStopwatch Create()
    {
        return new StopwatchImpl();
    }
}