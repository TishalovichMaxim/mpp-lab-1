namespace AssemblyScannerLib;

using System.Reflection;

public class AssemblyScanner
{
    private List<MethodInfo> _extensionMethods = new();

    void ProcessType(NamespaceInfo namespaceInfo, Type t)
    {
        if (t.IsStaticClass())
        {
            List<MethodInfo> staticMethods = t.GetExtensionMethods();
            _extensionMethods.AddRange(staticMethods);
        }

        string? fullName = t.FullName;
        if (fullName is null)
        {
            return;
        }

        string[] parts = fullName.Split('.');

        NamespaceInfo currNamespaceInfo = namespaceInfo;

        for (int i = 0; i < parts.Length - 1; i++)
        { 
            if (currNamespaceInfo.NestedNamespaces.ContainsKey(parts[i]))
            {
                currNamespaceInfo = currNamespaceInfo.NestedNamespaces[parts[i]];
            }
            else
            {
                NamespaceInfo temp = currNamespaceInfo; 
                currNamespaceInfo = new NamespaceInfo();
                temp.NestedNamespaces[parts[i]] = currNamespaceInfo;
            }
        }

        currNamespaceInfo.Types[parts.Last()] = new TypeInfo(t);
    }

    private void ProcessExtensionMethod(NamespaceInfo root, MethodInfo methodInfo)
    {
        Type extendedClass = methodInfo
                                .GetParameters()[0]
                                .ParameterType;

        string? nameSpace = extendedClass.FullName;
        if (nameSpace is null)
        {
            return;
        }

        string[] parts = nameSpace.Split('.');

        NamespaceInfo currNamespaceInfo = root;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (currNamespaceInfo.NestedNamespaces.ContainsKey(parts[i]))
            {
                currNamespaceInfo = currNamespaceInfo.NestedNamespaces[parts[i]];
            }
            else
            {
                return;
            }
        }

        TypeInfo extendedClassTypeInfo = currNamespaceInfo.Types[parts.Last()];
        extendedClassTypeInfo.ExtensionMethods.Add(new MethodData(methodInfo));
    }

    private void SetExtenstionMethods(NamespaceInfo root)
    {
        foreach (MethodInfo methodInfo in _extensionMethods)
        {
            ProcessExtensionMethod(root, methodInfo);
        }
    }

    public Dictionary<string, NamespaceInfo> Scan(string assemblyPath)
    {
        _extensionMethods.Clear();

        NamespaceInfo root = new();

        Assembly assembly = Assembly.LoadFile(assemblyPath);

        Type[] types = assembly.GetTypes();

        foreach (Type type in types)
        {
            ProcessType(root, type);
        }

        SetExtenstionMethods(root);

        return root.NestedNamespaces;
    }
}

