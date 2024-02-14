using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Tracer;

public class ThreadTrace
{
    public int ThreadId
    { get; }
    
    public long Time
    { get; }

    private List<MethodTrace> _methodTraces;

    public IReadOnlyList<MethodTrace> MethodTraces => _methodTraces;

    public ThreadTrace(int threadId, long time, List<MethodTrace> methodTraces)
    {
        ThreadId = threadId;
        Time = time;
        _methodTraces = methodTraces;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ThreadTrace))
        {
            return false;
        }

        ThreadTrace threadTrace = (ThreadTrace)obj;
        return ThreadId == threadTrace.ThreadId
               && Time == threadTrace.Time
               && MethodTraces.SequenceEqual(threadTrace.MethodTraces);
    }

    public override int GetHashCode()
    {
        int hash = 13;

        unchecked
        {
            hash = 7 * hash + ThreadId;
            hash = 7 * hash + Time.GetHashCode();
            
            foreach (MethodTrace methodTrace in MethodTraces)
            {
                hash = unchecked((7 * hash) + methodTrace.GetHashCode());
            }
        }

        return hash;
    }
}