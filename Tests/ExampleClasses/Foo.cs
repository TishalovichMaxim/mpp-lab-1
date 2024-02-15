using Tracer;

namespace Tests.ExampleClasses;

public class Foo
{
    private Bar _bar;
    private ITracer _tracer;

    internal Foo(ITracer tracer)
    {
        _tracer = tracer;
        _bar = new Bar(_tracer);
    }

    public void MyMethod()
    {
        _tracer.StartTrace();
        
        Console.WriteLine("Before _bar.InnerMethod()");
        
        _bar.InnerMethod();
        
        Console.WriteLine("After _bar.InnerMethod()");
        
        _tracer.StopTrace();
    }
}