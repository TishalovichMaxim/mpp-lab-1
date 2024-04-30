namespace DependencyInjectionContainer;

[AttributeUsage(AttributeTargets.Parameter)]
public class DependencyKey(string qualifier) : Attribute
{
    public string Qualifier = qualifier;
}