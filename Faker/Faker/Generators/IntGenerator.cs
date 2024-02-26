namespace DtoGenerator.Generators;

public class IntGenerator : IGenerator
{
    private readonly Random _random = new Random();
    
    // generates random values from [Int32.MinValue + 1; Int32.MaxValue]
    // mb i need to change it to generate full Int32 range
    public object Generate()
    {
        return _random.Next() - _random.Next();
    }
}