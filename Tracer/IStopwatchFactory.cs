using System.Diagnostics;

namespace Tracer;

public interface IStopwatchFactory
{
    IStopwatch Create();
}