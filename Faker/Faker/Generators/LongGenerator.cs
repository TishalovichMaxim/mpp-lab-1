namespace DtoGenerator.Generators;

public class LongGenerator : IGenerator
{
    private readonly Random _random = new Random();
    
    public object Generate()
    {
        return _random.NextInt64() - _random.NextInt64();
    }
}