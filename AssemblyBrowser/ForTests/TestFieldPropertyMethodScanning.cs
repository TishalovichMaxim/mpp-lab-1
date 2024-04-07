using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTests;

public class TestFieldPropertyMethodScanning
{
    public int a;

    public string b
    { get; set; } = "Init value";

    public string C()
    {
        return "Some string";
    }
}
