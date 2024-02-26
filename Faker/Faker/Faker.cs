using System.Reflection;
using DtoGenerator.Generators;

namespace DtoGenerator;

public class Faker
{
    private readonly Dictionary<Type, IGenerator> _generators = new();
    
    public Faker()
    {
        _generators[typeof(int)] = new IntGenerator();
        _generators[typeof(long)] = new LongGenerator();
    }
    
    public bool IsDto(Type t)
    {
        Attribute? attribute = t.GetCustomAttribute(typeof(DtoAttribute));
        return attribute is not null;
    }
    
    public ConstructorInfo GetConstructor(Type t)
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

    public FieldInfo[] GetPublicFields(Type t)
    {
        return t.GetFields(BindingFlags.Instance | BindingFlags.Public);
    }

    public PropertyInfo[] GetPublicProperties(Type t)
    {
        return t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
    }
}