using System.Reflection;

namespace AssemblyScannerLib;

public struct MethodData
{
    public string Name;

    public Type ReturnType;

    public List<Type> Params;

    public bool IsStatic;

    public bool IsAbstract;

    public bool IsSealed;

    public bool IsVirtual;

    public MethodAttributes AccessModifier;

    public MethodData(MethodInfo methodInfo)
    {
        IsStatic = methodInfo.IsStatic;
        IsAbstract = methodInfo.IsAbstract;
        IsSealed = methodInfo.IsFinal;
        IsVirtual = methodInfo.IsVirtual;

        Params = methodInfo.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToList();

        Name = methodInfo.Name;

        ReturnType = methodInfo.ReturnType;

        if (methodInfo.IsPublic)
        {
            AccessModifier = MethodAttributes.Public;
        }
        else if (methodInfo.IsFamilyOrAssembly)
        {
            AccessModifier = MethodAttributes.FamORAssem;
        } 
        else if (methodInfo.IsAssembly)
        {
            AccessModifier = MethodAttributes.Assembly;
        } 
        else if (methodInfo.IsFamily)
        {
            AccessModifier = MethodAttributes.Family;
        }
        else if (methodInfo.IsFamilyAndAssembly)
        {
            AccessModifier = MethodAttributes.FamANDAssem;
        }
        //else if (methodInfo.IsPrivate)
        else
        {
            AccessModifier = MethodAttributes.Private;
        }
    }
}
