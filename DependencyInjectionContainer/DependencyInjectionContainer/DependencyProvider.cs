using System.Reflection;
using System.Runtime.CompilerServices;

namespace DependencyInjectionContainer;

public class DependencyProvider
{
    private IDictionary<Type, IList<GenerationInfo>> _config;

    private Dictionary<Type, object> _singletonObjects = new();

    private object? TryGenerateObject(ConstructorInfo constructorInfo)
    {
        ParameterInfo[] paramInfos = constructorInfo.GetParameters();

        List<object> parameters = new();

        foreach (ParameterInfo paramInfo in paramInfos)
        {
            object? param = Generate(paramInfo.ParameterType);
            if (param == null)
            {
                return null;
            }
            parameters.Add(param);
        }

        return constructorInfo.Invoke(parameters.ToArray());
    }

    private object Generate(Type t)
    {
        ConstructorInfo[] constructors = t.GetConstructors();

        foreach (ConstructorInfo constructorInfo in constructors)
        {
            object? res = TryGenerateObject(constructorInfo);
            if (res != null)
            {
                return res;
            }
        }

        throw new DiException($"It's impossible to implement {t}");
    }

    private object ResolveDependencyByGenerationInfo(GenerationInfo info)
    {
        object? res;
        if (info.GenerationType == GenerationType.SINGLETON)
        {
            if (!_singletonObjects.TryGetValue(info.Source, out res))
            {
                res = Generate(info.Source);
                _singletonObjects[info.Source] = res;
            }
        }
        else
        {
            res = Generate(info.Source);
        }

        return res;
    }

    private object GenerateAll(Type target, IList<GenerationInfo> implementations)
    {
        Type t = typeof(List<>);
        Type newT = t.MakeGenericType([target]);

        MethodInfo? method = newT.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

        ConstructorInfo? constructor = newT.GetConstructor([]);
        object l = constructor!.Invoke([]);

        foreach (GenerationInfo implementation in implementations)
        {
            method!.Invoke(l, [ResolveDependencyByGenerationInfo(implementation)]);
        }

        return l;
    }
    
    private object? Resolve(Type t)
    {
        IList<GenerationInfo>? infoList;
        if (!_config.TryGetValue(t, out infoList))
        {
            if (t.GetGenericTypeDefinition() != typeof(IEnumerable<>))
            {
                throw new DiException($"It's impossible to implement {t}");
            }

            if (t.GetGenericArguments().Length != 1)
            {
                throw new DiException($"It's impossible to implement {t}");
            }

            Type paramType = t.GetGenericArguments()[0];

            if (!_config.TryGetValue(paramType, out var implementations))
            {
                throw new DiException($"It's impossible to implement {t}");
            }

            return GenerateAll(paramType, implementations);
        }

        if (infoList.Count > 1)
        {
            throw new DiException($"{infoList.Count} possible implementations for {t}");
        }

        GenerationInfo info = infoList[0];

        return ResolveDependencyByGenerationInfo(info);
    }
    
    public T Resolve<T>()
    {
        return (T) Resolve(typeof(T));
    }

    public DependencyProvider(DependenciesConfiguration config)
    {
        ConfigurationValidator validator = new();
        validator.Validate(config);

        _config = config.mapper;
    }
}
