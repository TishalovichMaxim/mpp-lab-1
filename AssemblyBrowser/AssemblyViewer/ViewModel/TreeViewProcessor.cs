using System.Reflection;
using System.Text;
using System.Windows.Controls;
using AssemblyScannerLib;
using TypeInfo = AssemblyScannerLib.TypeInfo;

namespace AssemblyViewer.ViewModel;

public class TreeViewProcessor
{
    private static readonly Dictionary<MethodAttributes, string> _accessModifiers = new()
    {
        { MethodAttributes.Public, "public" },
        { MethodAttributes.FamORAssem, "protected internal" },
        { MethodAttributes.Assembly, "internal" },
        { MethodAttributes.Family, "protected" },
        { MethodAttributes.FamANDAssem, "protected private" },
        { MethodAttributes.Private, "private" }
    };

    private TreeViewItem ProcessFieldData(FieldData fieldData)
    {
        TreeViewItem res = new TreeViewItem();

        string name = fieldData.Name;

        string type = fieldData.Type.Name;

        res.Header = $"field: {type} {name}";

        return res;
    }

    private TreeViewItem ProcessPropertyData(PropertyData propertyData)
    {
        TreeViewItem res = new TreeViewItem();

        string name = propertyData.Name;

        string type = propertyData.Type.Name;

        res.Header = $"property: {type} {name}";

        return res;
    }

    private TreeViewItem ProcessMethodData(MethodData methodData, bool isExtension = false)
    {
        TreeViewItem res = new TreeViewItem();

        string name = methodData.Name;

        string accessModifier = _accessModifiers[methodData.AccessModifier];

        string returnValueTypeName = methodData.ReturnType.Name;

        StringBuilder sb = new StringBuilder();

        sb.Append(accessModifier);
        sb.Append(" ");

        if (methodData.IsStatic)
        {
            sb.Append("static ");
        }

        if (methodData.IsVirtual)
        {
            sb.Append("virtual ");
        }

        if (methodData.IsAbstract)
        {
            sb.Append("abstract ");
        }

        if (methodData.IsSealed)
        {
            sb.Append("sealed ");
        }

        sb.Append(returnValueTypeName);
        sb.Append(" ");

        sb.Append(name);
        sb.Append("(");

        if (isExtension)
        {
            sb.Append("this ");
        }

        foreach(Type paramType in methodData.Params)
        {
            sb.Append(paramType.Name);
            sb.Append(", ");
        }

        if (methodData.Params.Count != 0)
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

        foreach (FieldData fieldData in typeInfo.Fields)
        {
            curr.Items.Add(ProcessFieldData(fieldData));
        }

        foreach (PropertyData propertyData in typeInfo.Properties)
        {
            curr.Items.Add(ProcessPropertyData(propertyData));
        }

        foreach (MethodData methodData in typeInfo.Methods)
        {
            curr.Items.Add(ProcessMethodData(methodData));
        }

        foreach (MethodData methodData in typeInfo.ExtensionMethods)
        {
            curr.Items.Add(ProcessMethodData(methodData, true));
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
