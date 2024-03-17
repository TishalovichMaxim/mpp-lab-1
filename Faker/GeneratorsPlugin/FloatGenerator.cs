using DtoGenerator;
using DtoGenerator.Generator;

namespace GeneratorsPlugin;

[Generator(typeof(float))]
public class FloatGenerator : IGenerator
{
    private readonly Random _random;

    public FloatGenerator()
    {
        _random = new Random();
    }

    public object Generate(Type t, Faker faker)
    {
        return _random.NextSingle();
    }
}
