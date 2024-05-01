using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Classes;

internal class FastTrain : ITrain
{
    public string Name
    { get; } = "SomeFastTrain";

    public int Speed
    { get; } = 90;

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(FastTrain)) 
        {
            return false;
        }

        FastTrain fastTrain = (FastTrain)obj;
        return fastTrain.Name.Equals(Name)
            && fastTrain.Speed.Equals(Speed);
    }

    public override int GetHashCode()
    {
        int hash = 13;

        unchecked
        {
            hash = 7 * hash + Name.GetHashCode();
            hash = 7 * hash + Speed.GetHashCode();
        }

        return hash;
    }
}
