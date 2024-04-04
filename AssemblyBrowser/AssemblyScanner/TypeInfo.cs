using System.Reflection;

namespace AssemblyScannerLib;

public class TypeInfo
{
    public string TypeName;

    public List<MethodInfo> Methods;

    public List<PropertyInfo> Properties;

    public List<FieldInfo> Fields;

    public List<MethodInfo> ExtensionMethods;

    public TypeInfo(Type t)
    {
        TypeName = t.Name;

        Methods = t.GetNotExtensionMethods();

        Properties = t.GetProperties().ToList();

        Fields = t.GetFields().ToList();

        ExtensionMethods = new List<MethodInfo>();
    }
}
