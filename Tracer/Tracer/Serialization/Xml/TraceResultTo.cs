using YAXLib.Attributes;
using YAXLib.Enums;

namespace Tracer.Serialization.Xml;

[YAXSerializeAs("root")]
public class TraceResultTo
{
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement,
        EachElementName="thread")]
    public List<ThreadTraceTo> threadTraces
    { get; set; }
    
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
        threadTraces = new List<ThreadTraceTo>();
    }

    internal TraceResult TraceResult =>
        new(
            (from threadTrace in threadTraces 
                select threadTrace.ThreadTrace).ToList()
        );
}
