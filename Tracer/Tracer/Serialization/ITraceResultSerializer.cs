namespace Tracer.Serialization;

public interface ITraceResultSerializer
{
    string Serialize(TraceResult traceResult);
    
    TraceResult? Deserialize(string content);
}