using System.Reflection;

namespace AssemblyScannerLib;

internal static class TypeExtension
{
    public static bool IsStaticClass(this Type t)
    {
        return t.IsAbstract && t.IsSealed;
    }

    public static List<MethodInfo> GetExtensionMethods(this Type t)
    {
        return (from method in t.GetRuntimeMethods()
               where method.IsExtensionMethod()
               select method).ToList();
    }

    public static List<MethodInfo> GetNotExtensionMethods(this Type t)
    {
        return (from method in t.GetRuntimeMethods()
               where !method.IsExtensionMethod()
               select method).ToList();
    }
}
