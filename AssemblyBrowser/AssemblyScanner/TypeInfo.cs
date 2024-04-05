using System.Reflection;

namespace AssemblyScannerLib;

public class TypeInfo
{
    public string TypeName;

    public List<MethodData> Methods;

    public List<PropertyData> Properties;

    public List<FieldData> Fields;

    public List<MethodData> ExtensionMethods;

    public TypeInfo(Type t)
    {
        TypeName = t.Name;

        Methods = t.GetNotExtensionMethods()
            .Select(methodInfo => new MethodData(methodInfo))
            .ToList();

        Properties = t.GetProperties()
            .Select(propertyInfo => new PropertyData(propertyInfo))
            .ToList();

        Fields = t.GetFields()
            .Select(fieldInfo => new FieldData(fieldInfo))
            .ToList();

        ExtensionMethods = new List<MethodData>();
    }
}
