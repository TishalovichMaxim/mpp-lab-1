using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer;

public class DiException : Exception
{
    public DiException(string message) : base(message)
    {
    }
} 
