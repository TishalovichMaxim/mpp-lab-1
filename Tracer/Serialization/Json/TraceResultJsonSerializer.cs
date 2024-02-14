using System.Xml;

namespace Tracer.Serialization.Json;

using System.Runtime.Serialization.Json;
using System.Text;

public class TraceResultJsonSerializer : ITraceResultSerializer
{
    public string Serialize(TraceResult traceResult)
    {
        TraceResultTo traceResultTo = new(traceResult); 
        
        using Stream stream = new MemoryStream();

        using var writer = JsonReaderWriterFactory
            .CreateJsonWriter(stream, Encoding.UTF8, true, true, "    ");

        DataContractJsonSerializer dataContractJsonSerializer
            = new DataContractJsonSerializer(typeof(TraceResultTo));
        dataContractJsonSerializer.WriteObject(writer, traceResultTo);
        writer.Flush();

        StreamReader streamReader = new StreamReader(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return streamReader.ReadToEnd();
    }

    public TraceResult? Deserialize(string content)
    {
        Byte[] bytes = Encoding.UTF8.GetBytes(content);
        using XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(
                bytes,
                0,
                bytes.Length,
                Encoding.UTF8,
                new XmlDictionaryReaderQuotas(),
                null
                );

        DataContractJsonSerializer serializer = new(typeof(TraceResultTo));
        TraceResultTo? traceResultTo = (TraceResultTo?)serializer.ReadObject(reader);

        return traceResultTo?.TraceResult;
    }
}