using FakerGenerator;

namespace DtoGenerator.Generator;

[Generator(typeof(long))]
public class LongGenerator : IGenerator
{
    private readonly Random _random = new Random();
    
    public object Generate()
    {
        return _random.NextInt64() - _random.NextInt64();
    }
}