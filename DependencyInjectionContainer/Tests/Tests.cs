using DependencyInjectionContainer;
using FluentAssertions;
using Tests.Classes;

namespace Tests;

[TestClass]
public class Tests
{
    [TestMethod]
    public void CorrectInjection()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ITrain, FastTrain>();

        DependencyProvider provider = new(dependencies);

        var firstDependency = (FastTrain) provider.Resolve<ITrain>();

        firstDependency.Should().Be(new FastTrain());
    }

    [TestMethod]
    public void SingletonInjection()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ClassWithDefaultEquals, ClassWithDefaultEquals>();

        DependencyProvider provider = new(dependencies);

        var firstDependency = provider.Resolve<ClassWithDefaultEquals>();
        var secondDependency = provider.Resolve<ClassWithDefaultEquals>();

        firstDependency.Should().Be(secondDependency);
    }

    [TestMethod]
    public void InstancePerDependencyInjection()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ClassWithDefaultEquals, ClassWithDefaultEquals>(GenerationType.InstPerDep);

        DependencyProvider provider = new(dependencies);

        var firstDependency = provider.Resolve<ClassWithDefaultEquals>();
        var secondDependency = provider.Resolve<ClassWithDefaultEquals>();

        firstDependency.Should().NotBe(secondDependency);
    }

    [TestMethod]
    public void InjectIEnumerable()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ITrain, FastTrain>(GenerationType.InstPerDep, "fast");
        dependencies.Register<ITrain, SlowTrain>(GenerationType.InstPerDep, "slow");

        DependencyProvider provider = new(dependencies);
        
        IEnumerable<ITrain> trains = provider.Resolve<IEnumerable<ITrain>>();

        HashSet<Type> actual = new();
        actual.UnionWith(trains.Select<ITrain, Type>(t => t.GetType()));

        HashSet<Type> expected = new();
        expected.Add(typeof(FastTrain));
        expected.Add(typeof(SlowTrain));

        actual.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void DefaultTemplateRegistration()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<IRepository, MySqlRepository>();
        dependencies.Register<IService<IRepository>, ServiceImpl<IRepository>>();

        DependencyProvider provider = new(dependencies);

        IService<IRepository> service = provider.Resolve<IService<IRepository>>();
    }

    [TestMethod]
    public void OpenGenerics()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<IRepository, MySqlRepository>();
        dependencies.Register(typeof(IService<>), typeof(ServiceImpl<>));

        DependencyProvider provider = new(dependencies);
        
        IService<IRepository> service = provider.Resolve<IService<IRepository>>();
    }

    [TestMethod]
    public void DependencyQualifier()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ITrain, FastTrain>(GenerationType.InstPerDep, "fast");
        dependencies.Register<ITrain, SlowTrain>(GenerationType.InstPerDep, "slow");

        DependencyProvider provider = new(dependencies);

        ITrain train = provider.Resolve<ITrain>("fast");

        train.GetType().Should().Be<FastTrain>();
    }

    [TestMethod]
    public void ConstructorDependencyQualifier()
    {
        DependenciesConfiguration dependencies = new();
        dependencies.Register<ITrain, FastTrain>(GenerationType.InstPerDep, "fast");
        dependencies.Register<ITrain, SlowTrain>(GenerationType.InstPerDep, "slow");
        dependencies.Register<Railway, Railway>();

        DependencyProvider provider = new(dependencies);

        Railway railway = provider.Resolve<Railway>();

        railway.Train.GetType().Should().Be<FastTrain>();
    }

    [TestMethod]
    public void Check()
    {
        Type t1 = typeof(IService<>);
        Type t2 = typeof(ServiceImpl<>);
    }
}