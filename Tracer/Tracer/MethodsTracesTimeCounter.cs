namespace Tracer;

public class MethodsTracesTimeCounter
{
    public long Count(IEnumerable<MethodTrace> methodTraces)
    {
        long res = 0;
        
        foreach (var methodTrace in methodTraces)
        {
            res += methodTrace.Milliseconds;
            res += Count(methodTrace.NestedMethodTraces);
        }

        return res;
    }
}