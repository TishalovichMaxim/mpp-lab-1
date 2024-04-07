using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.AccessControl;

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

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) {
            return false;
        }

        MethodData md = (MethodData)obj;
        return Name.Equals(md.Name)
            && ReturnType.Equals(md.ReturnType)
            && Params.SequenceEqual(md.Params)
            && IsStatic == md.IsStatic
            && IsAbstract == md.IsAbstract
            && IsSealed == md.IsSealed
            && IsVirtual == md.IsVirtual
            && AccessModifier.Equals(md.AccessModifier);
    }

    public override int GetHashCode()
    {
        int hash = 13;

        unchecked
        {
            hash = 7 * hash + Name.GetHashCode();
            hash = 7 * hash + ReturnType.GetHashCode();

            foreach (Type paramType in Params)
            {
                hash = unchecked((7 * hash) + paramType.GetHashCode());
            }

            hash = 7 * hash + IsStatic.GetHashCode();
            hash = 7 * hash + IsAbstract.GetHashCode();
            hash = 7 * hash + IsSealed.GetHashCode();
            hash = 7 * hash + IsVirtual.GetHashCode();
            hash = 7 * hash + AccessModifier.GetHashCode();
        }

        return hash;
    }
}
