using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

internal class TestClassInfo
{
    public string ClassName;

    public string Content;

    public TestClassInfo(string className, string content)
    {
        ClassName = className;
        Content = content;
    }
}
