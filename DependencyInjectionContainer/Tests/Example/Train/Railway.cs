using DependencyInjectionContainer;

namespace Tests.Classes;

internal class Railway
{
    public ITrain Train;
    
    public Railway([DependencyKey("fast")] ITrain train)
    {
        Train = train;
    }
}