using System.Reflection;
using DtoGenerator;
using DtoGenerator.Config;
using DtoGenerator.Generator;
using FluentAssertions;
using Tests.ExampleClasses;

namespace Tests;

[TestClass]
public class UnitTest
{
    [TestMethod]
    public void TestCyclicDependency()
    {
        Faker faker = new Faker(new GeneratorsLoader(), new FakerConfig());
        Person person = faker.Create<Person>();

        person.Mom.Should().BeNull();
        person.Dad.Should().BeNull();
    }

    
}