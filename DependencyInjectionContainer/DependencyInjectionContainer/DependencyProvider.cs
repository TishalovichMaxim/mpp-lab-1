using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DependencyInjectionContainer;

public class DependencyProvider
{
    private IDictionary<Type, IList<GenerationInfo>> _config;

    private ConcurrentDictionary<Type, object> _singletonObjects = new();

    private object? OpenGenericTest(Type t)
    {
        if (!t.IsGenericType)
        {
            return null;
        }

        Type genericType = t.GetGenericTypeDefinition();
        Type[] genericArguments = t.GetGenericArguments();

        IList<GenerationInfo>? implementations;
        if (!_config.TryGetValue(genericType, out implementations))
        {
            return null;
        }

        if (implementations.Count != 1)
        {
            throw new DiException("There are several open generic implementations...");
        }

        GenerationInfo generationInfo = new GenerationInfo(
            implementations[0].Source.MakeGenericType(genericArguments),
            implementations[0].GenerationType 
        );
        
        return ResolveDependencyByGenerationInfo(t, generationInfo);
    }
    
    private object? TryGenerateObject(ConstructorInfo constructorInfo)
    {
        ParameterInfo[] paramInfos = constructorInfo.GetParameters();

        List<object> parameters = new();

        foreach (ParameterInfo paramInfo in paramInfos)
        {
            DependencyKey? attribute = paramInfo.GetCustomAttribute<DependencyKey>();
            object? param = Resolve(paramInfo.ParameterType, attribute?.Qualifier);
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

    private object ResolveDependencyByGenerationInfo(Type target, GenerationInfo info)
    {
        if (info.GenerationType == GenerationType.Singleton)
        {
            return _singletonObjects.GetOrAdd(target, Generate(info.Source));
        }
        
        return Generate(info.Source);
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
            method!.Invoke(l, [ResolveDependencyByGenerationInfo(target, implementation)]);
        }

        return l;
    }
    
    private object? Resolve(Type t, string? qualifier)
    {
        IList<GenerationInfo>? infoList;
        if (!_config.TryGetValue(t, out infoList))
        {
            object? res = OpenGenericTest(t);
            if (res != null)
            {
                return res;
            }

            if (!t.IsGenericType)
            {
                throw new DiException($"It's impossible to implement {t}");
            }
                
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
            if (qualifier == null)
            {
                throw new DiException($"{infoList.Count} possible implementations for {t}");
            }
            
            IList<GenerationInfo> qualifiedGenInfo = infoList.Where(info => qualifier.Equals(info.Qualifier)).ToList();
            if (qualifiedGenInfo.Count != 1)
            {
                throw new DiException($"There is no suitable dependency with qualifier {qualifier}");
            }

            return ResolveDependencyByGenerationInfo(t, qualifiedGenInfo.First());
        }

        GenerationInfo info = infoList[0];

        return ResolveDependencyByGenerationInfo(t, info);
    }
    
    public T Resolve<T>(string? qualifier = null)
    {
        return (T) Resolve(typeof(T), qualifier);
    }

    public DependencyProvider(DependenciesConfiguration config)
    {
        ConfigurationValidator validator = new();
        validator.Validate(config);

        _config = config.mapper;
    }
}
