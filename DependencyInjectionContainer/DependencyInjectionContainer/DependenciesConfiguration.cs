using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

public class DependenciesConfiguration
{

    internal readonly Dictionary<Type, IList<GenerationInfo>> mapper = new(); 

    public void Register(Type target, Type source,
        GenerationType generationType = GenerationType.Singleton, string? qualifier = null)
    {
        if (!mapper.ContainsKey(target))
        {
            mapper[target] = new List<GenerationInfo>();
        }
        
        mapper[target].Add(new GenerationInfo(source, generationType, qualifier));
    }

    public void Register<T, S>(GenerationType generationType = GenerationType.Singleton, string? qualifier = null)
    {
        Register(typeof(T), typeof(S), generationType, qualifier);
    }

    public void Register<T, S>(string? qualifier)
    {
        Register(typeof(T), typeof(S), GenerationType.Singleton, qualifier);
    }
}

