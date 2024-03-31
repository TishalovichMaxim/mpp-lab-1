namespace AssemblyScannerLib;

using System.Reflection;

public class AssemblyScanner
{
    private void ProcessType(NamespaceInfo namespaceInfo, Type t)
    {
        string? nameSpace = t.Namespace;
        if (nameSpace is null)
        {
            return;
        }

        string[] parts = nameSpace.Split('.');

        NamespaceInfo currNamespaceInfo = namespaceInfo;

        foreach (string part in parts)
        { 
            if (currNamespaceInfo.NestedNamespaces.ContainsKey(part))
            {
                currNamespaceInfo = currNamespaceInfo.NestedNamespaces[part];
            }
            else
            {
                NamespaceInfo temp = currNamespaceInfo; 
                currNamespaceInfo = new NamespaceInfo();
                temp.NestedNamespaces.Add(part, currNamespaceInfo);
            }
        }

        currNamespaceInfo.Types.Add(new TypeInfo(t));
    }

    public NamespaceInfo Scan(string assemblyPath)
    {
        NamespaceInfo res = new();

        Assembly assembly = Assembly.LoadFile(assemblyPath);

        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            ProcessType(res, type);
        }

        return res;
    }
}

