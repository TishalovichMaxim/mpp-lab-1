namespace Tests.Classes;

public class MySqlRepository : IRepository
{
    public void store(object o)
    {
        Console.WriteLine(o);
    }
}