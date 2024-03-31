using System.Reflection;

namespace AssemblyScannerLib;

public class TypeInfo
{
    public string TypeName;

    public List<MethodInfo> Methods;

    public List<PropertyInfo> Properties;

    public List<FieldInfo> Fields;

    public TypeInfo(string typeName)
    {
        Methods = new List<MethodInfo>();
        Properties = new List<PropertyInfo>();
        Fields = new List<FieldInfo>();

        TypeName = typeName;
    }

    public TypeInfo(Type t)
    {
        TypeName = t.Name;

        Methods = t.GetMethods().ToList();
        Properties = t.GetProperties().ToList();
        Fields = t.GetFields().ToList();
    }
}
