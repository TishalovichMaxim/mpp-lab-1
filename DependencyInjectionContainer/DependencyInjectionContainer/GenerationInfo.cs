using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

internal struct GenerationInfo
{

    Type Source
    { get; }

    GenerationType GenerationType
    { get; }

    public GenerationInfo(Type source, GenerationType generationType)
    {
        Source = source;
        GenerationType = generationType;
    }
}
