using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct ParameterInfo
{
    string Type;
    
    string Name;

    public ParameterInfo(string type, string name)
    {
        Type = type;

        Name = name;
    }
}
