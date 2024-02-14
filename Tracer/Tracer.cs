namespace Tracer;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

public class Tracer : ITracer
{
    private readonly IStopwatchFactory _stopwatchFactory;
    
    private ConcurrentDictionary<int, IList<MethodMeter>> ThreadTraceInfos
    {
        get;
    } = new();

    private ConcurrentDictionary<int, List<MethodTrace>> ThreadMethodTraces
    {
        get;
    } = new();

    public Tracer(IStopwatchFactory stopwatchFactory)
    {
        _stopwatchFactory = stopwatchFactory;
    }
    
    private MethodBase GetCurrentMethodBase()
    {
        StackTrace st = new(2); //current trace is skipped
        StackFrame? sf = st.GetFrame(0); //the first frame is the trace method
        if (sf == null)
        {
            throw new TracerException("Method stack trace is null");
        }
        
        MethodBase? mb = sf.GetMethod();
        if (mb == null)
        {
            throw new TracerException("Method base is null");
        }

        return mb;
    }

    public void StartTrace()
    {
        int threadId = Environment.CurrentManagedThreadId;

        if (!ThreadTraceInfos.ContainsKey(threadId))
        {
            ThreadTraceInfos[threadId] = new List<MethodMeter>();
            ThreadMethodTraces[threadId] = new List<MethodTrace>();
        }

        MethodBase methodBase = GetCurrentMethodBase();
        MethodMeter curr = new(methodBase, _stopwatchFactory.Create());
        curr.StartTimer();
        ThreadTraceInfos[threadId].Add(curr);
    }

    public void StopTrace()
    {
        int threadId = Thread.CurrentThread.ManagedThreadId;

        IList<MethodMeter> methodTraceInfos = ThreadTraceInfos[threadId];
        int index = methodTraceInfos.Count - 1;
        MethodMeter methodMeter = methodTraceInfos[index];
        methodMeter.StopTimer();

        methodTraceInfos.RemoveAt(index);
        MethodTrace currMethodTrace = methodMeter.MethodTrace;

        if (index == 0)
        {
            ThreadMethodTraces[threadId].Add(currMethodTrace);
        }
        else
        {
            MethodMeter parent = methodTraceInfos[index - 1];
            parent.AddMethodTrace(currMethodTrace);
        }
    }

    public TraceResult GetTraceResult()
    {
        List<ThreadTrace> threadTracesInfos = new List<ThreadTrace>();
        MethodsTracesTimeCounter methodsTracesTimeCounter = new();

        foreach (var kvp in ThreadMethodTraces)
        {
            long time = methodsTracesTimeCounter.Count(kvp.Value);
            ThreadTrace threadTrace = new(kvp.Key, time, kvp.Value);
            threadTracesInfos.Add(threadTrace);
        }
        
        return new TraceResult(threadTracesInfos);
    }
}