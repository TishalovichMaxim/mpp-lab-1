namespace Tests.ExampleClasses;

using Tracer;

public class Bar
{
    private ITracer _tracer;

    internal Bar(ITracer tracer)
    {
        _tracer = tracer;
    }

    public void InnerMethod()
    {
        _tracer.StartTrace();
        Console.WriteLine("In Bar.InnerMethod()");
        _tracer.StopTrace();
    }
}