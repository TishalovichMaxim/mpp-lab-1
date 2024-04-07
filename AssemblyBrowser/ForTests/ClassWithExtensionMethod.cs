using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTests;

internal static class ClassWithExtensionMethod
{
    public static string ExtensionMethod(this ExtendedClass e)
    {
        return e.ToString();
    }
}
