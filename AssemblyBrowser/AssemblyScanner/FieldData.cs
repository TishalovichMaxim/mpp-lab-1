using System.Reflection;

namespace AssemblyScannerLib;

public struct FieldData
{
    public string Name;

    public Type Type;

    public FieldData(FieldInfo fieldInfo)
    {
        Name = fieldInfo.Name;
        Type = fieldInfo.FieldType;
    }
}
