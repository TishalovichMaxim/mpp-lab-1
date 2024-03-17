namespace DtoGenerator.Generator;

[Generator(typeof(string))]
public class StringGenerator : IGenerator
{
    public object Generate(Type t, Faker faker)
    {
        return "aboba";
    }
}