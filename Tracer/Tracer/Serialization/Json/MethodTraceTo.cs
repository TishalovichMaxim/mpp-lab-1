using System.Runtime.Serialization;

namespace Tracer.Serialization.Json;

[DataContract]
public class MethodTraceTo
{
    private long _time;
    
    [DataMember(Name = "name", Order = 1)]
    public string MethodName;

    [DataMember(Name = "class", Order = 2)]
    public string ClassName;

    [DataMember(Name = "time", Order = 3)]
    public string TimeValue
    {
        get => _time + "ms";
        set => _time = long.Parse(value[..^2]);
    }
    
    [DataMember(Name = "methods", Order = 4)]
    public List<MethodTraceTo> NestedTraces;
    
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
    }

    public MethodTrace MethodTrace =>
        new(
            MethodName,
            _time,
            ClassName,
            (from nestedTrace in NestedTraces
                select nestedTrace.MethodTrace).ToList()
        );
}