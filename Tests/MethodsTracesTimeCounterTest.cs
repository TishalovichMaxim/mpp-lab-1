namespace Tests;

[TestClass]
public class MethodsTracesTimeCounterTest
{
    private readonly MethodsTracesTimeCounter _timeCounter = new();
    
    [TestMethod]
    public void Count_EmptyEnumerable_ReturnZero()
    {
        IEnumerable<MethodTrace> enumerable = new List<MethodTrace>();
        _timeCounter.Count(enumerable)
            .Should()
            .Be(0);
    }
    
    [TestMethod]
    public void Count_RecursiveEnumerable()
    {
        MethodTrace methodTrace1 = new(
            "aboba",
            10,
            "Some",
            []
        );

        MethodTrace methodTrace2 = new(
            "aboba",
            20,
            "Some",
            []
        );

        MethodTrace methodTrace3 = new(
            "aboba",
            30,
            "Some",
            []
        );

        MethodTrace methodTrace4 = new(
            "name",
            40,
            "className",
            [methodTrace1, methodTrace2]
            );

        MethodTrace methodTrace5 = new(
            "aboba",
            50,
            "Some",
            []
        );
        
        MethodTrace methodTrace6 = new(
            "name",
            60,
            "className",
            [methodTrace5]
            );        
        
        MethodTrace methodTrace7 = new(
            "name",
            70,
            "className",
            [methodTrace6]
            );

        List<MethodTrace> methodTraces =
        [
            methodTrace3,
            methodTrace4,
            methodTrace7
        ];

        _timeCounter.Count(methodTraces)
            .Should()
            .Be(280);
    }
}