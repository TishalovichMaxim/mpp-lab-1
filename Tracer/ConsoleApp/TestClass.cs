using System.ServiceModel;

namespace ConsoleApp;

[XmlSerializerFormat]
public class TestClass
{
    public IList<string> _list;

    public TestClass(IList<string> list)
    {
        _list = list;
    }

    public TestClass()
    {
        
    }
}