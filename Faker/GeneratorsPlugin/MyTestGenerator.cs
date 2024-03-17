using DtoGenerator;
using DtoGenerator.Generator;

namespace GeneratorsPlugin;

[Generator(typeof(string))]
public class MyTestGenerator : IGenerator
{
    public object Generate(Type type, Faker faker)
    {
        return "Some Test Value";
    }
}
