using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ForTests;

internal class TestAccessModifiersScanning
{
    private int a()
    {
        return 0;
    }

    private protected int b()
    {
        return 0;
    }

    internal int c()
    {
        return 0;
    }

    protected int d()
    {
        return 0;
    }

    protected internal int e()
    {
        return 0;
    }

    public int f()
    {
        return 0;
    }
}
