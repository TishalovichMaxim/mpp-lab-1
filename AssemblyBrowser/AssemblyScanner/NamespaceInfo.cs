using System;
namespace AssemblyScannerLib;

public class NamespaceInfo
{
    public Dictionary<string, NamespaceInfo> NestedNamespaces = new();

    public List<TypeInfo> Types = new();
}
