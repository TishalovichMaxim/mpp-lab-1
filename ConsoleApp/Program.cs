using ConsoleApp.Output;
using Tracer.Serialization;
using Tracer.Serialization.Json;
using Tracer.Serialization.Xml;

namespace ConsoleApp;

using Tracer;

class Program
{
    private static readonly IStopwatchFactory StopwatchFactory = new StopwatchFactory();
    private static readonly Tracer Tracer = new(StopwatchFactory);

    private static void Hello(string name)
    {
        Console.WriteLine("Hello, " + name);
    }

    private static void FirstThreadStart()
    {
        Tracer.StartTrace();
        
        Thread.Sleep(200);
        Hello("Vasya");
        
        Tracer.StopTrace();
    }
 
    private static void SecondThreadStart()
    {
        Tracer.StartTrace();
        
        Hello("Petya");
        
        FirstThreadStart();
        
        Tracer.StopTrace();
    }
       
    public static void Main(string[] args)
    {
        Thread thread1 = new(FirstThreadStart);
        Thread thread2 = new(SecondThreadStart);
        
        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
        
        TraceResult traceResult = Tracer.GetTraceResult();

        IWriter writer = new ConsoleWriter();
        ITraceResultSerializer serializer = new TraceResultXmlSerializer();
        
        TraceResultWriter traceResultWriter = new();
        traceResultWriter.Write(traceResult, serializer, writer);
    }
}