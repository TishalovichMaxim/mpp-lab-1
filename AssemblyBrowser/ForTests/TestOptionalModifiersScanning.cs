using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ForTests;

internal abstract class TestOptionalModifiersScanning
{
    public static string StaticMethod()
    {
        return "This is static method";
    }

    public virtual string VirtualMethod()
    {
        return "This is virtual method";
    }

    public sealed override string ToString()
    {
        return "This is sealed method";
    }

    public abstract string AbstractMethod();
}
