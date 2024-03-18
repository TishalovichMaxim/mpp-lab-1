using DtoGenerator.Generator;
using DtoGenerator;

namespace Tests.ExampleGenerators;

[Generator(typeof(string))]
public class NameGenerator : IGenerator
{
    public object Generate(Type type, Faker faker)
    {
        return "SomeRandomName";
    }
}
