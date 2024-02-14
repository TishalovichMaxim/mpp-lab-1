using YAXLib.Attributes;
using YAXLib.Enums;

namespace Tracer.Serialization.Xml;

public class ThreadTraceTo
{
    private long _time;

    public int Id;

    [YAXAttributeForClass]
    [YAXSerializeAs("id")]
    public string IdStringValue
    {
        get => Id.ToString();
        set => Id = int.Parse(value);
    }
    
    [YAXAttributeForClass]
    [YAXSerializeAs("time")]
    public string TimeValue
    {
        get => _time + "ms";
        set => _time = long.Parse(value[..^2]);
    }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement,
        EachElementName = "method")]
    public List<MethodTraceTo> MethodsTraces
    { get; set; }
    
    public ThreadTraceTo(ThreadTrace threadTrace)
    {
        FromThreadTrace(threadTrace);
    }

    public ThreadTraceTo()
    {
        MethodsTraces = new List<MethodTraceTo>();
    }

    public void FromThreadTrace(ThreadTrace threadTrace)
    {
        _time = threadTrace.Time;
        Id = threadTrace.ThreadId;
        _time = threadTrace.Time;
        
        MethodsTraces = new List<MethodTraceTo>();
        foreach (MethodTrace methodTrace in threadTrace.MethodTraces)
        {
            MethodsTraces.Add(new MethodTraceTo(methodTrace));
        }
    }
    
    internal ThreadTrace ThreadTrace =>
        new(
            Id,
            _time,
            (from methodTrace in MethodsTraces
                select methodTrace.MethodTrace).ToList()
        );
}
