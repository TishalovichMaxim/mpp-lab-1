using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

internal class ConfigurationValidator
{
    private void ValidatePair(Type target, Type source)
    {
        if (source.IsAbstract)
        {
            throw new DiException("Implementation type cannot be abstract!");
        }

        if (target.IsValueType)
        {
            throw new DiException("Target type should be reference type!");
        }
    }

    public void Validate(DependenciesConfiguration configuration)
    {
        foreach (Type target in configuration.mapper.Keys)
        {
            foreach (GenerationInfo info in configuration.mapper[target])
            {
                ValidatePair(target, info.Source);
            }
        }
    }
}
