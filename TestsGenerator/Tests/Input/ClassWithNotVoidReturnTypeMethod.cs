
namespace Tests.Input;

public class ClassWithNotVoidReturnTypeMethod
{
    public ClassWithNotVoidReturnTypeMethod(IDependency1 dependency1, IDependency2 dependency2)
    {
        Console.WriteLine("Constructor");
    }
    
    public string MyMethod(string a, int b)
    {
        return "Aboba";
    }
}