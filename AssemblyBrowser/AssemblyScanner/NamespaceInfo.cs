using System;
namespace AssemblyScannerLib;

public class NamespaceInfo
{
    public Dictionary<string, NamespaceInfo> NestedNamespaces = new();

    public Dictionary<string, TypeInfo> Types = new();
}
