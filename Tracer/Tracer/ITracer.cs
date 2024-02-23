namespace Tracer;

public interface ITracer
{
    void StartTrace();
    
    void StopTrace();
    
    TraceResult GetTraceResult();
}