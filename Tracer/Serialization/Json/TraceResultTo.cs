using System.Runtime.Serialization;

namespace Tracer.Serialization.Json;

[DataContract]
public class TraceResultTo
{
    [DataMember(Name = "threads")]
    public List<ThreadTraceTo> threadTraces;
    
    public TraceResultTo(TraceResult traceResult)
    {
        threadTraces = new List<ThreadTraceTo>();
        foreach (ThreadTrace threadTrace in traceResult.ThreadTracesInfos)
        {
            threadTraces.Add(new ThreadTraceTo(threadTrace));
        }
    }

    public TraceResultTo()
    {
    }

    public TraceResult TraceResult =>
        new(
            (from threadTrace in threadTraces 
                select threadTrace.ThreadTrace).ToList()
        );
}