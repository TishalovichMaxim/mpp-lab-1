using FakerGenerator;

namespace DtoGenerator.Generator;

[Generator(typeof(string))]
public class StringGenerator : IGenerator
{
    public object Generate()
    {
        return "aboba";
    }
}