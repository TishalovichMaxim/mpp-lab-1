namespace ConsoleApp.Output;

public class ConsoleWriter : IWriter
{
    public void Write(string content)
    {
        Console.WriteLine(content);
    }
}