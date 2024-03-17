using System.Reflection;

namespace DtoGenerator.Generator;

public class ListGenerator : IGenerator
{
    private static readonly int SIZE = 5;

    public List<T> _genenerateList<T>(int count, Faker faker)
    {
        List<T> l = new List<T>();
        for (int i = 1; i < count + 1; i++)
        {
            l.Add(faker.Create<T>());
        }
        return l;
    } 

    public object Generate(Type t, Faker faker)
    {
        Type baseType = t.GenericTypeArguments[0];

        MethodInfo methodInfo = typeof(ListGenerator).GetMethod("_genenerateList")!;

        MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(baseType);
        object[] args = [SIZE, faker];

        return genericMethodInfo.Invoke(this, args)!;
    }
}
