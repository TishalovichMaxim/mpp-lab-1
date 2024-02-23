using System.Xml;
using YAXLib;

namespace Tracer.Serialization.Xml;

public class TraceResultXmlSerializer : ITraceResultSerializer
{
    public string Serialize(TraceResult traceResult)
    {
        TraceResultTo traceResultTo = new(traceResult);
        
        using Stream stream = new MemoryStream();
        
        XmlWriterSettings settings = new()
            { Indent = true, IndentChars = "    ", OmitXmlDeclaration = true };
 
        using XmlWriter writer = XmlWriter.Create(stream, settings);

        YAXSerializer serializer = new(typeof(TraceResultTo));
        serializer.Serialize(traceResultTo, writer);
        
        writer.Flush();
        
        StreamReader streamReader = new(stream);
        stream.Seek(0, SeekOrigin.Begin);
        
        string result = streamReader.ReadToEnd();
        return result.Replace(" />", "/>");
    }

    public TraceResult? Deserialize(string content)
    {
        YAXSerializer serializer = new(typeof(TraceResultTo));
        TraceResultTo? traceResultTo = (TraceResultTo?)serializer.Deserialize(content);
        return traceResultTo?.TraceResult;
    }
}