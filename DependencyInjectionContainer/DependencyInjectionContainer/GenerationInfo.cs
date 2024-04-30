using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

internal struct GenerationInfo
{
    public Type Source
    { get; }

    public GenerationType GenerationType
    { get; }
    
    public string? Qualifier
    { get; }

    public GenerationInfo(Type source, GenerationType generationType, string? qualifier = null)
    {
        Source = source;
        GenerationType = generationType;
        Qualifier = qualifier;
    }
}
