using YAXLib.Attributes;
using YAXLib.Enums;

namespace Tracer.Serialization.Xml;

public class MethodTraceTo
{
    private long _time;
    
    [YAXAttributeForClass]
    [YAXSerializeAs("name")]
    public string MethodName
    { get; set; }

    [YAXAttributeForClass]
    [YAXSerializeAs("time")]
    public string TimeValue
    {
        get => _time + "ms";
        set => _time = long.Parse(value[..^2]);
    }
    
    [YAXAttributeForClass]
    [YAXSerializeAs("class")]
    public string ClassName
    { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement,
        EachElementName="method")]
    public List<MethodTraceTo> NestedTraces
    { get; set; }
    
    public MethodTraceTo(MethodTrace methodTrace)
    {
        _time = methodTrace.Milliseconds;
        ClassName = methodTrace.ClassName;
        MethodName = methodTrace.Name;

        NestedTraces = new List<MethodTraceTo>();
        foreach (MethodTrace trace in methodTrace.MethodTraces)
        {
            NestedTraces.Add(new MethodTraceTo(trace));
        }
    }

    public MethodTraceTo()
    {
        NestedTraces = new List<MethodTraceTo>();
    }

    internal MethodTrace MethodTrace =>
        new(
            MethodName,
            _time,
            ClassName,
            (from nestedTrace in NestedTraces ??= []
                select nestedTrace.MethodTrace).ToList()
        );
}
