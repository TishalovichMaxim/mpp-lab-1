using System.Reflection;

namespace Tracer;

public class MethodMeter
{
    private readonly IStopwatch _stopwatch;

    private readonly string _methodName;

    private readonly string _methodClassName;

    private readonly List<MethodTrace> _nestedTraces = new();

    private MethodTrace? _methodTrace = null;

    internal MethodTrace MethodTrace =>
        _methodTrace ??= new MethodTrace(
            _methodName,
            _stopwatch.GetElapsedMilliseconds(),
            _methodClassName,
            _nestedTraces);

    public MethodMeter(MethodBase methodBase, IStopwatch stopwatch)
    {
        _methodName = methodBase.Name;
        Type? methodClassType = methodBase.DeclaringType;
        if (methodClassType == null)
        {
            throw new TracerException("Method class type is null");
        }
        
        _methodClassName = methodClassType.Name;
        _stopwatch = stopwatch;
    }

    public void StartTimer()
    {
        _stopwatch.Start();
    }

    public void StopTimer()
    {
        _stopwatch.Stop();
    }

    internal void AddMethodTrace(MethodTrace methodTrace)
    {
        _nestedTraces.Add(methodTrace);
    }
}