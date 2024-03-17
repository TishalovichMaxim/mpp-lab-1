using DtoGenerator;
using DtoGenerator.Generator;

namespace GeneratorsPlugin;

public class DoubleGenerator : IGenerator
{
    private readonly Random _random;

    public DoubleGenerator()
    {
        _random = new Random();
    }

    public object Generate(Type t, Faker faker)
    {
        return _random.NextDouble();
    }
}
