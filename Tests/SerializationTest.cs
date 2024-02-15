using Tracer.Serialization.Json;
using Tracer.Serialization.Xml;

namespace Tests;

[TestClass]
public class SerializationTest
{
    public TraceResult GetMultiThreadedTraceResult()
    {
        MethodTrace methodTrace1 = new(
            "FirstMethod",
            10,
            "FirstClass",
            []
        );
        
        MethodTrace methodTrace2 = new(
            "SecondMethod",
            20,
            "SecondClass",
            []
        );
        
        MethodTrace methodTrace3 = new(
            "ThirdMethod",
            30,
            "ThirdClass",
            [methodTrace2]
        );

        MethodTrace methodTrace4 = new(
            "FourthMethod",
            40,
            "FourthClass",
            []
        );

        MethodTrace methodTrace5 = new(
            "FifthMethod",
            50,
            "FifthClass",
            []
        );

        MethodTrace methodTrace6 = new(
            "SixthMethod",
            60,
            "SixthClass",
            [methodTrace5]
        );

        ThreadTrace threadTrace1 = new(
            1,
            60,
            [methodTrace1, methodTrace3]
        );

        ThreadTrace threadTrace2 = new(
            2,
            150,
            [methodTrace6, methodTrace4]
        );

        return new TraceResult([threadTrace1, threadTrace2]);
    }
    
    [TestMethod]
    public void MultiThreaded_TraceResult_Json_Serialization()
    {
        TraceResult traceResult = GetMultiThreadedTraceResult();

        TraceResultJsonSerializer serializer = new();

        string actual = serializer.Serialize(traceResult);

        string expected = File.ReadAllText("TestFiles/multithreaded_trace_result.json");

        actual.Should().Be(expected);
    }
    
    [TestMethod]
    public void MultiThreaded_TraceResult_Xml_Serialization()
    {
        TraceResult traceResult = GetMultiThreadedTraceResult();

        TraceResultXmlSerializer serializer = new();

        string actual = serializer.Serialize(traceResult);
        Console.WriteLine(actual);

        string expected = File.ReadAllText("TestFiles/multithreaded_trace_result.xml");
        Console.WriteLine(expected);

        actual.Should().Be(expected);
    }
    
    [TestMethod]
    public void MultiThreaded_TraceResult_Json_Deserialization()
    {
        TraceResultJsonSerializer serializer = new();
        
        string content = File.ReadAllText("TestFiles/multithreaded_trace_result.json");
        TraceResult? deserializedTraceResult = serializer.Deserialize(content);
        
        deserializedTraceResult
            .Should().Be(GetMultiThreadedTraceResult());
    }
    
    [TestMethod]
    public void MultiThreaded_TraceResult_Xml_Deserialization()
    {
        TraceResultXmlSerializer serializer = new();
        
        string content = File.ReadAllText("TestFiles/multithreaded_trace_result.xml");
        TraceResult? deserializedTraceResult = serializer.Deserialize(content);
        
        deserializedTraceResult
            .Should().Be(GetMultiThreadedTraceResult());
    }
}