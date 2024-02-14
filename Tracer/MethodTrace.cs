using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Tracer;

public class MethodTrace
{
    public string Name
    { get; }

    public long Milliseconds
    { get; }
    
    public string ClassName
    { get; }

    internal List<MethodTrace> MethodTraces
    { get; }

    public IReadOnlyList<MethodTrace> NestedMethodTraces => MethodTraces;
    
    public MethodTrace(string name, long milliseconds, string className, List<MethodTrace> methodTraces)
    {
        Name = name;
        Milliseconds = milliseconds;
        ClassName = className;
        MethodTraces = methodTraces;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        MethodTrace methodTrace = (MethodTrace)obj;
        return Name.Equals(methodTrace.Name)
               && Milliseconds == methodTrace.Milliseconds
               && ClassName.Equals(methodTrace.ClassName)
               && MethodTraces.SequenceEqual(methodTrace.MethodTraces);
    }

    public override int GetHashCode()
    {
        int hash = 13;

        unchecked
        {
            hash = 7 * hash + Name.GetHashCode();
            hash = 7 * hash + Milliseconds.GetHashCode();
            hash = 7 * hash + ClassName.GetHashCode();
            
            foreach (MethodTrace methodTrace in MethodTraces)
            {
                hash = unchecked((7 * hash) + methodTrace.GetHashCode());
            }
        }

        return hash;
    }
}