using System.Reflection;
using DtoGenerator;
using DtoGenerator.Config;
using DtoGenerator.Generator;
using FluentAssertions;
using Tests.ExampleClasses;

namespace Tests;

[TestClass]
public class FakerTests
{
    [TestMethod]
    public void TestCyclicDependency()
    {
        Faker faker = new Faker(new GeneratorsLoader(), new FakerConfig());
        Person person = faker.Create<Person>();

        person.Mom.Should().BeNull();
        person.Dad.Should().BeNull();
    }

    [TestMethod]    
    public void TestLoadingPlugins()
    {
        Faker faker = new Faker(new GeneratorsLoader(), new FakerConfig());
        faker.LoadPlugins([
            "C:\\Users\\tisha\\BSUIR\\sem6\\Mpp\\Labs\\Faker\\GeneratorsPlugin\\bin\\Debug\\net8.0\\GeneratorsPlugin.dll"
            ]);

        string generatedValue = faker.Create<string>();

        generatedValue.Should().Be("Some Test Value");
    }
}