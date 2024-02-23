namespace ConsoleApp.Output;

public class FileWriter : IWriter
{
    private readonly string _fileName;
    
    public void Write(string content)
    {
        using StreamWriter sw = File.CreateText(_fileName);
        
        sw.WriteLine(content);
    }

    public FileWriter(string fileName)
    {
        _fileName = fileName;
    }
}