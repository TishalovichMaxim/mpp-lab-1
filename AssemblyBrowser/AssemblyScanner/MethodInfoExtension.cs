using System.Reflection;
using System.Runtime.CompilerServices;

namespace AssemblyScannerLib;

internal static class MethodInfoExtension
{
    public static bool IsExtensionMethod(this MethodInfo methodInfo)
    {
        return methodInfo.IsDefined(typeof(ExtensionAttribute));
    }
}
