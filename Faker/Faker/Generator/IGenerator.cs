namespace DtoGenerator.Generator;

public interface IGenerator
{
    object Generate(Type t, Faker faker);
}
