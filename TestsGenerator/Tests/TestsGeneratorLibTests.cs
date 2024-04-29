using FluentAssertions;
using TestsGeneratorLib;

namespace Tests;

[TestClass]
public class TestsGeneratorLibTests
{
    private static readonly TestsGenerator TestsGenerator = new();

    private static readonly string OutputDirectory = "./Output";

    [TestMethod]
    public void TestSmartClassWithNonVoidReturnTypeMethod()
    {
        string expected = File.ReadAllText("./Resources/class_with_not_void_return_type_method.txt")
            .Replace("\r", "");
        
        TestsGenerator.Generate(
            [
                "./Input/ClassWithNotVoidReturnTypeMethod.cs"
            ],
           OutputDirectory,
           5,
           5,
           5
        ).Wait();

        string actual = File.ReadAllText(OutputDirectory + "/ClassWithNotVoidReturnTypeMethodTests.cs")
            .Replace("\r", "");

        actual.Should().Be(expected);
    }

    [TestMethod]
    public void TestSmartClassWithVoidReturnTypeMethod()
    {
        string expected = File.ReadAllText("./Resources/class_with_void_return_type_method.txt")
            .Replace("\r", "");
        
        TestsGenerator.Generate(
            [
                "./Input/ClassWithVoidReturnTypeMethod.cs"
            ],
           OutputDirectory,
           5,
           5,
           5
        ).Wait();

        string actual = File.ReadAllText(OutputDirectory + "/ClassWithVoidReturnTypeMethodTests.cs")
            .Replace("\r", "");

        actual.Should().Be(expected);
    }

    [TestMethod]
    public void TestNotSmartClass()
    {
        string expected = File.ReadAllText("./Resources/not_smart_class.txt")
            .Replace("\r", "");
        
        TestsGenerator.Generate(
            [
                "./Input/NotSmartClass.cs"
            ],
           OutputDirectory,
           5,
           5,
           5
        ).Wait();

        string actual = File.ReadAllText(OutputDirectory + "/NotSmartClassTests.cs")
            .Replace("\r", "");

        actual.Should().Be(expected);
    }
}