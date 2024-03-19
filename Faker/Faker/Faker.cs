using System.Reflection;
using DtoGenerator.Config;
using DtoGenerator.Generator;

namespace DtoGenerator;

public class Faker
{
    private readonly Dictionary<Type, IGenerator> _generators = new();

    private readonly ISet<Type> _generatedTypes = new HashSet<Type>();

    private readonly GeneratorsLoader _loader;

    private readonly Dictionary<Type, Dictionary<(string, Type), IGenerator>> _configGenerators;

    public Faker(GeneratorsLoader loader, FakerConfig fakerConfig)
    {
        _loader = loader;

        _generators[typeof(int)] = new IntGenerator();
        _generators[typeof(long)] = new LongGenerator();
        _generators[typeof(Uri)] = new UriGenerator();
        _generators[typeof(List<>)] = new ListGenerator();
        _generators[typeof(string)] = new StringGenerator();

        _configGenerators = fakerConfig.ConfigGenerators;
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
            string? paramName = parameterInfo.Name;
            if (paramName == null)
            {
                continue;
            }

            (string name, Type type) paramKey = (paramName, parameterInfo.ParameterType);
            paramKey.name = paramKey.name[..1].ToUpper() + paramKey.name[1..];

            if (_configGenerators.ContainsKey(t)
                && _configGenerators[t].ContainsKey(paramKey))
            {
                IGenerator generator = _configGenerators[t][paramKey];
                constructorParameters.Add(generator.Generate(t, this));
            }
            else
            {
                constructorParameters.Add(Create(parameterInfo.ParameterType)!);
            }
        }

        object res = constructorInfo.Invoke(constructorParameters.ToArray());
        
        FieldInfo[] fieldsInfos = GetPublicFields(t);
        foreach (FieldInfo fieldInfo in fieldsInfos)
        {
            (string, Type) paramKey = (fieldInfo.Name, fieldInfo.FieldType);

            if (_configGenerators.ContainsKey(t)
                && _configGenerators[t].ContainsKey(paramKey))
            {
                IGenerator generator = _configGenerators[t][paramKey];
                fieldInfo.SetValue(res, generator.Generate(t, this));
            }
            else
            {
                fieldInfo.SetValue(res, Create(fieldInfo.FieldType));
            }

        }

        PropertyInfo[] propertiesInfos = GetPublicProperties(t);
        foreach (PropertyInfo propertyInfo in propertiesInfos)
        {
            (string, Type) paramKey = (propertyInfo.Name, propertyInfo.PropertyType);

            if (_configGenerators.ContainsKey(t)
                && _configGenerators[t].ContainsKey(paramKey))
            {
                IGenerator generator = _configGenerators[t][paramKey];
                propertyInfo.SetValue(res, generator.Generate(t, this));
            }
            else
            {
                propertyInfo.SetValue(res, Create(propertyInfo.PropertyType));
            }
        }

        return res;
    }

    private object? CreateNotDto(Type concreteType)
    {
        Type type = concreteType;
        if (concreteType.IsGenericType)
        {
            type = concreteType.GetGenericTypeDefinition();
        }

        if (_generators.ContainsKey(type))
        {
            IGenerator generator = _generators[type];
            return generator.Generate(concreteType, this);
        }

        return default;
    }
    
    internal object? Create(Type t)
    {
        if (!_generatedTypes.Add(t))
        {
            return CreateNotDto(t);
        }

        object? res;
        if (IsDto(t))
        {
            res = CreateDto(t);
        }
        else
        {
            res = CreateNotDto(t);
        }

        _generatedTypes.Remove(t);
        return res;
    }
    
    public void LoadPlugins(string[] plugins)
    {
        foreach (string plugin in plugins)
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
        return (T)Create(typeof(T))!;
    }
}