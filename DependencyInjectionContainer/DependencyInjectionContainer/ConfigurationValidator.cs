using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

internal class ConfigurationValidator
{
    private bool CheckInterface(Type interfaceType, Type openGeneric)
    {
        if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == openGeneric)
        {
            return true;
        }

        return interfaceType
            .GetInterfaces()
            .Any(i => CheckInterface(i, openGeneric));
    }

    private void ValidatePair(Type target, Type source)
    {
        if (target.IsValueType || source.IsValueType)
        {
            throw new DiException("Dependency and Implementation types should be reference types!");
        }
        
        if (source.IsAbstract)
        {
            throw new DiException("Implementation type cannot be abstract!");
        }

        if (source.IsAssignableTo(target))
        {
            return;
        }

        if (target.IsGenericTypeDefinition && source.IsGenericTypeDefinition)
        {
            if (source == target)
            {
                return;
            }
        
            if (target.IsClass)
            {
                Type curr = source.BaseType!;
                while (curr != typeof(object))
                {
                    if (curr.IsGenericType && curr.GetGenericTypeDefinition() == target)
                    {
                        return;
                    }

                    curr = source.BaseType!;
                }
            }
            else if (source.GetInterfaces().Any(i => CheckInterface(i, target)))
            {
                return;
            }
        }
        
        throw new DiException("Implementation type should be compatible with dependency type!");
    }

    private void ValidateCompatibility(DependenciesConfiguration configuration)
    {
        foreach (Type target in configuration.mapper.Keys)
        {
            foreach (GenerationInfo info in configuration.mapper[target])
            {
                ValidatePair(target, info.Source);
            }
        }
    }

    private void ValidateAmbiguous(DependenciesConfiguration configuration)
    {
        foreach (Type target in configuration.mapper.Keys)
        {
            HashSet<string?> qualifiers = new();
            
            foreach (GenerationInfo info in configuration.mapper[target])
            {
                if (!qualifiers.Add(info.Qualifier))
                {
                    throw new DiException($"There are several implementations of {target} with equal qualifier!");
                }
            }
        }
    }

    public void Validate(DependenciesConfiguration configuration)
    {
        ValidateCompatibility(configuration);
        
        ValidateAmbiguous(configuration);
        
        
    }
}
