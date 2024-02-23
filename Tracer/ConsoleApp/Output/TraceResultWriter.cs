using Tracer;
using Tracer.Serialization;

namespace ConsoleApp.Output;

public class TraceResultWriter
{
    public void Write(TraceResult traceResult,
        ITraceResultSerializer traceResultSerializer,
        IWriter writer)
    {
        string content = traceResultSerializer.Serialize(traceResult);
        writer.Write(content);
    }
}