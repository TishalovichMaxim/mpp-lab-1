using System.Reflection;

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

    private object? Resolve(Type t)
    {
        IList<GenerationInfo>? infoList;
        if (!_config.TryGetValue(t, out infoList))
        {
            throw new DiException($"It's impossible to implement {t}");
        }

        if (infoList.Count > 1)
        {
            throw new DiException($"{infoList.Count} possible implementations for {t}");
        }

        GenerationInfo info = infoList[0];
        
        object res;
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
