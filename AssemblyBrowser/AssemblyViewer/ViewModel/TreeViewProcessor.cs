using System.Reflection;
using System.Text;
using System.Windows.Controls;
using AssemblyScannerLib;
using TypeInfo = AssemblyScannerLib.TypeInfo;

namespace AssemblyViewer.ViewModel;

public class TreeViewProcessor
{
    private TreeViewItem ProcessFieldInfo(FieldInfo fieldInfo)
    {
        TreeViewItem res = new TreeViewItem();

        string name = fieldInfo.Name;

        string type = fieldInfo.GetType().Name;

        res.Header = $"field: {type} {name}";

        return res;
    }

    private TreeViewItem ProcessPropertyInfo(PropertyInfo propertyInfo)
    {
        TreeViewItem res = new TreeViewItem();

        string name = propertyInfo.Name;

        string type = propertyInfo.GetType().Name;

        res.Header = $"property: {type} {name}";

        return res;
    }

    private TreeViewItem ProcessMethodInfo(MethodInfo methodInfo)
    {
        TreeViewItem res = new TreeViewItem();

        string name = methodInfo.Name;

        string accessLevel = "";

        if (methodInfo.IsPublic)
        {
            accessLevel = "public";
        }
        else if (methodInfo.IsFamilyOrAssembly)
        {
            accessLevel = "protected internal";
        } 
        else if (methodInfo.IsAssembly)
        {
            accessLevel = "internal";
        } 
        else if (methodInfo.IsFamily)
        {
            accessLevel = "protected";
        }
        else if (methodInfo.IsFamilyAndAssembly)
        {
            accessLevel = "protected private";
        }
        else if (methodInfo.IsPrivate)
        {
            accessLevel = "private";
        }

        string returnValueTypeName = methodInfo.ReturnType.Name;

        StringBuilder sb = new StringBuilder();

        sb.Append(accessLevel);
        sb.Append(" ");

        if (methodInfo.IsStatic)
        {
            sb.Append("static ");
        }

        if (methodInfo.IsVirtual)
        {
            sb.Append("virtual ");
        }

        if (methodInfo.IsAbstract)
        {
            sb.Append("abstract ");
        }

        if (methodInfo.IsFinal)
        {
            sb.Append("sealed ");
        }

        sb.Append(returnValueTypeName);
        sb.Append(" ");

        sb.Append(name);
        sb.Append("(");

        ParameterInfo[] parameters = methodInfo.GetParameters();
        foreach(ParameterInfo parameterInfo in parameters)
        {
            sb.Append(parameterInfo.ParameterType.Name);
            sb.Append(", ");
        }

        if (parameters.Length != 0)
        {
            sb.Remove(sb.Length - 2, 2);
        }

        sb.Append(")");

        res.Header = sb.ToString();

        return res;
    }

    private TreeViewItem ProcessTypeInfo(TypeInfo typeInfo)
    {
        TreeViewItem curr = new();

        curr.Header = typeInfo.TypeName;

        foreach (FieldInfo fieldInfo in typeInfo.Fields)
        {
            curr.Items.Add(ProcessFieldInfo(fieldInfo));
        }

        foreach (PropertyInfo propertyInfo in typeInfo.Properties)
        {
            curr.Items.Add(ProcessPropertyInfo(propertyInfo));
        }

        foreach (MethodInfo methodInfo in typeInfo.Methods)
        {
            curr.Items.Add(ProcessMethodInfo(methodInfo));
        }

        foreach (MethodInfo methodInfo in typeInfo.ExtensionMethods)
        {
            curr.Items.Add(ProcessMethodInfo(methodInfo));
        }

        return curr;
    }

    private TreeViewItem ProcessNamespaceInfo(string name, NamespaceInfo namespaceInfo)
    {
        TreeViewItem curr = new();
        curr.Header = name;

        foreach (string namespaceName in namespaceInfo.NestedNamespaces.Keys)
        {
            NamespaceInfo currNamespaceInfo = namespaceInfo.NestedNamespaces[namespaceName];
            TreeViewItem nested = ProcessNamespaceInfo(namespaceName, currNamespaceInfo);
            curr.Items.Add(nested);
        }

        foreach (TypeInfo typeInfo in namespaceInfo.Types.Values)
        {
            TreeViewItem nested = ProcessTypeInfo(typeInfo);
            curr.Items.Add(nested);
        }

        return curr;
    }

    public List<TreeViewItem> Process(Dictionary<string, NamespaceInfo> namespaces)
    {
        List<TreeViewItem> res = new();

        foreach (string namespaceName in namespaces.Keys)
        {
            res.Add(ProcessNamespaceInfo(namespaceName, namespaces[namespaceName]));
        }

        return res;
    }
}
