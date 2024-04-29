namespace Tests.Input;

public class NotSmartClass
{
    public NotSmartClass(IDependency1 dependency1, int a)
    {
        
    }

    public IDependency1? DoSomething(IDependency2 dependency2)
    {
        Console.WriteLine("DoSOmehitng");
        return null;
    }
}