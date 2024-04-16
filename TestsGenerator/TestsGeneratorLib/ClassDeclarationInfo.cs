﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct ClassDeclarationInfo
{
    public string ClassName;

    public IList<string> MethodsNames;

    public ClassDeclarationInfo(string className, IList<string> methodsNames)
    {
        ClassName = className;
        MethodsNames = methodsNames;
    }
}
