namespace Tracer;

public class TraceResult
{
    private readonly List<ThreadTrace> _threadTraces;

    public IReadOnlyList<ThreadTrace> ThreadTracesInfos => _threadTraces;
    
    public TraceResult(List<ThreadTrace> threadTraces)
    {
        _threadTraces = threadTraces;
    }
    
    public TraceResult()
    {
        _threadTraces = new List<ThreadTrace>();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(TraceResult))
        {
            return false;
        }

        TraceResult traceResult = (TraceResult)obj;
        return _threadTraces.SequenceEqual(traceResult._threadTraces);
    }

    public override int GetHashCode()
    {
        int hash = 13;
        
        foreach (ThreadTrace threadTraceInfo in _threadTraces)
        {
            hash = unchecked((7 * hash) + threadTraceInfo.GetHashCode());
        }
        
        return hash;
    }
}