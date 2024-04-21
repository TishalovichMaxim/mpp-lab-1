using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct FieldInfo
{
    public string Type;

    public string Name;

    public FieldInfo(string type, string name)
    {
        Name = name;
        Type = type;
    }
}

