namespace Tests.Input;

public class ClassWithVoidReturnTypeMethod
{
    public ClassWithVoidReturnTypeMethod(IDependency1 dependency1, IDependency2 dependency2)
    {
        Console.WriteLine("Constructor");
    }
    
    public void MyMethod(string a, int b)
    {
        Console.WriteLine("Aboba");
    }
}