using System.Reflection;

namespace DtoGenerator.Generator;

public class GeneratorsLoader
{
    internal bool IsGenerator(Type t)
    {
        string? generatorInterfacePath = typeof(IGenerator).FullName;
        if (generatorInterfacePath == null )
        {
            return false;
        }

        Type? type = t.GetInterface(generatorInterfacePath);
        if (type == null)
        {
            return false;
        }

        Attribute? attribute = t.GetCustomAttribute(typeof(GeneratorAttribute));
        return attribute != null;
    }

    public Dictionary<Type, IGenerator> LoadGenerators(string pluginAssemblyPath)
    {
        Dictionary<Type, IGenerator> loadedGenerators = new();

        Assembly pluginAssembly = Assembly.LoadFile(pluginAssemblyPath);
        Type[] expectedPlugins = pluginAssembly.GetTypes();
        foreach (Type expectedPlugin in expectedPlugins)
        {
            if (IsGenerator(expectedPlugin))
            {
                GeneratorAttribute generatorAttribute = (GeneratorAttribute)expectedPlugin
                    .GetCustomAttribute(typeof(GeneratorAttribute))!;
                Type generatedValueType = generatorAttribute.GenereatorType;
                IGenerator? generator = Activator.CreateInstance(expectedPlugin) as IGenerator;
                loadedGenerators[generatedValueType] = generator!;
            }
        }

        return loadedGenerators;
    }
}
