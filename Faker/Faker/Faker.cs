using System.Reflection;
using DtoGenerator.Config;
using DtoGenerator.Generator;
using FakerGenerator;

namespace DtoGenerator;

public class Faker
{
    private readonly Dictionary<Type, IGenerator> _generators = new();

    private readonly ISet<Type> _generatedTypes = new HashSet<Type>();

    private readonly GeneratorsLoader _loader;

    private readonly Dictionary<>

    public Faker(GeneratorsLoader loader, FakerConfig fakerConfig)
    {
        _loader = loader;

        _generators[typeof(int)] = new IntGenerator();
        _generators[typeof(long)] = new LongGenerator();
    }

    private bool IsDto(Type t)
    {
        Attribute? attribute = t.GetCustomAttribute(typeof(DtoAttribute));
        return attribute is not null;
    }
    
    private FieldInfo[] GetPublicFields(Type t)
    {
        return t.GetFields(BindingFlags.Instance | BindingFlags.Public);
    }

    private PropertyInfo[] GetPublicProperties(Type t)
    {
        return t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    }
    
    private ConstructorInfo GetConstructor(Type t)
    {
        ConstructorInfo[] constructors = t.GetConstructors();
        ConstructorInfo chosenConstructor =  constructors[0];
        ParameterInfo[] parameterInfos = chosenConstructor.GetParameters();
        int maxParametersCount = parameterInfos.Length;

        for (int i = 1; i < parameterInfos.Length; i++)
        {
            parameterInfos = chosenConstructor.GetParameters();
            if (parameterInfos.Length > maxParametersCount)
            {
                maxParametersCount = parameterInfos.Length;
                chosenConstructor = constructors[i];
            }
        }

        return chosenConstructor;
    }

    private object CreateDto(Type t)
    {
        ConstructorInfo constructorInfo = GetConstructor(t);
        ParameterInfo[] parametersInfos = constructorInfo.GetParameters();
        List<object> constructorParameters = new();

        foreach (ParameterInfo parameterInfo in parametersInfos)
        {
            constructorParameters.Add(Create(parameterInfo.ParameterType));
        }

        object res = constructorInfo.Invoke(constructorParameters.ToArray());
        
        FieldInfo[] fieldsInfos = GetPublicFields(t);
        foreach (FieldInfo fieldInfo in fieldsInfos)
        {
            fieldInfo.SetValue(res, Create(fieldInfo.FieldType));
        }

        PropertyInfo[] propertiesInfos = GetPublicProperties(t);
        foreach (PropertyInfo propertyInfo in propertiesInfos)
        {
            propertyInfo.SetValue(res, Create(propertyInfo.PropertyType));
        }

        return res;
    }

    private object? CreateNotDto(Type t)
    {
        if (_generators.ContainsKey(t))
        {
            IGenerator generator = _generators[t];
            return generator.Generate();
        }

        return default;
    }
    
    private object? Create(Type t)
    {
        if (!_generatedTypes.Add(t))
        {
            return CreateNotDto(t);
        }

        if (IsDto(t))
        {
            return CreateDto(t);
        }

        return CreateNotDto(t);
    }
    
    public void LoadPlugins(String[] plugins)
    {
        foreach (String plugin in plugins)
        {
            Dictionary<Type, IGenerator> pluginGenerators = _loader.LoadGenerators(plugin);
            foreach (Type t in pluginGenerators.Keys)
            {
                _generators[t] = pluginGenerators[t];
            }
        }
    }

    public T Create<T>()
    {
        _generatedTypes.Clear();
        return (T)Create(typeof(T));
    }
}