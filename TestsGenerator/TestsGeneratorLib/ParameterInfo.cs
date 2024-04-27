using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct ParameterInfo
{
    public string Type;
    
    public string Name;

    public ParameterInfo(string type, string name)
    {
        Type = type;

        Name = name;
    }
}
