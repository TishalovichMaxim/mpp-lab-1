using System.Runtime.Serialization;

namespace Tracer.Serialization.Json;

[DataContract]
public class ThreadTraceTo
{
    private long _time;

    public int Id;

    [DataMember(Name = "id", Order = 1)]
    public string IdStringValue
    {
        get => Id.ToString();
        set => Id = int.Parse(value);
    }
    
    [DataMember(Name = "time", Order = 2)]
    public string TimeValue
    {
        get => _time + "ms";
        set => _time = long.Parse(value[..^2]);
    }

    [DataMember(Name = "methods", Order = 3)]
    public List<MethodTraceTo> MethodsTraces;
    
    public ThreadTraceTo(ThreadTrace threadTrace)
    {
        FromThreadTrace(threadTrace);
    }

    public ThreadTraceTo()
    {
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
    
    public ThreadTrace ThreadTrace =>
        new(
            Id,
            _time,
            (from methodTrace in MethodsTraces
                select methodTrace.MethodTrace).ToList()
        );
}