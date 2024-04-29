using TestsGeneratorLib;

namespace ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        TestsGenerator testsGenerator = new();

        Task task = testsGenerator.Generate(
            [
                @"C:\Users\tisha\BSUIR\sem6\Mpp\Labs\TestsGenerator\Tests\Input\ClassWithNotVoidReturnTypeMethod.cs",
                @"C:\Users\tisha\BSUIR\sem6\Mpp\Labs\TestsGenerator\Tests\Input\ClassWithVoidReturnTypeMethod.cs",
                @"C:\Users\tisha\BSUIR\sem6\Mpp\Labs\TestsGenerator\Tests\Input\NotSmartClass.cs",
            ],
            @"C:\Users\tisha\BSUIR\sem6\Mpp\Labs\TestsGenerator\Tests\Output",
            9,
            9,
            9
        );

        task.Wait();

        Console.WriteLine("Success");
    }
}