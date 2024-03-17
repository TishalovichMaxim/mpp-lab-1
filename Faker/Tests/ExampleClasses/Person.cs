using DtoGenerator;

namespace Tests.ExampleClasses;

[Dto]
public class Person
{
    public int Age { get; set; }

    public string Name { get; set; }

    public Person Mom
    {
        get;
        set;
    }

    public Person Dad
    {
        get;
        set;
    }

    public Person(string name, int age, Person mom, Person dad)
    {
        Name = name;
        Age = age;
        Dad = dad;
    }

    public Person(string name, int age, Person mom)
    {
        Name = name;
        Age = age;
    }

    public Person(string name, int age)
    {
        Name = name;
        Age = age;
    }

    public Person(string name)
    {
        Name = name;
    }

    public Person()
    {

    }
}