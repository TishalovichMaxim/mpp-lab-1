using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

public class DependenciesConfiguration
{

    internal readonly Dictionary<Type, GenerationInfo> mapper = new(); 

    public void Register(Type target, Type source,
        GenerationType generationType = GenerationType.SINGLETON)
    {
        mapper[target] = new GenerationInfo(source, generationType);
    }

    public void Register<T, S>(GenerationType generationType = GenerationType.SINGLETON)
    {
        Register(typeof(T), typeof(S));
    }

}

