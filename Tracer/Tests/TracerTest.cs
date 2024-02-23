using Tests.ExampleClasses;

namespace Tests;

[TestClass]
public class TracerTest
{
    private DefaultStopwatchFactory _defaultStopwatchFactory = new();
    
    private readonly FakeStopwatchFactory _fakeStopwatchFactory = new();

    private TraceResult GetExpectedTraceResult(int thread1Id, int thread2Id)
    {
        MethodTrace methodTrace1 = new(
            "InnerMethod",
            0L,
            "Bar",
            []
        );
        
        MethodTrace methodTrace2 = new(
            "MyMethod",
            0L,
            "Foo",
            [methodTrace1]
        );
        
        MethodTrace methodTrace3 = new(
            "InnerMethod",
            0L,
            "Bar",
            []
        );

        ThreadTrace threadTrace1 = new(
            thread1Id,
            0L,
            [methodTrace2]
        );
        
        ThreadTrace threadTrace2 = new(
            thread2Id,
            0L,
            [methodTrace3]
        );

        TraceResult res = new(
            [threadTrace1, threadTrace2]
        );

        return res;
    }
    
    [TestMethod]
    public void MultiThreadedTraceResult()
    {
        Tracer.Tracer tracer = new(_fakeStopwatchFactory);
        
        Foo foo = new(tracer);
        Bar bar = new(tracer);
        
        Thread thread1 = new(foo.MyMethod);
        Thread thread2 = new(bar.InnerMethod);
        
        thread1.Start();
        thread1.Join();
        
        thread2.Start();
        thread2.Join();

        TraceResult actualTraceResult = tracer.GetTraceResult();
        
        int thread1Id = thread1.ManagedThreadId;
        int thread2Id = thread2.ManagedThreadId;

        TraceResult expectedTraceResult = GetExpectedTraceResult(thread1Id, thread2Id);

        actualTraceResult
            .Should().Be(expectedTraceResult);
    }
}