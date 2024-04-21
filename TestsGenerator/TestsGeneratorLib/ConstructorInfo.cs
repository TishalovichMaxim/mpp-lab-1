using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct ConstructorInfo
{
    public List<ParameterInfo> Params;

    public ConstructorInfo(List<ParameterInfo> parameters)
    {
        Params = parameters;
    }
}
