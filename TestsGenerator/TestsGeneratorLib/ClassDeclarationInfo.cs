using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsGeneratorLib;

public struct ClassDeclarationInfo
{
    public string Namespace;
    
    public string ClassName;

    public IList<MethodDeclarationInfo> Methods;

    public ConstructorInfo? ConstructorInfo;

    public ClassDeclarationInfo(string namepsace, string className, IList<MethodDeclarationInfo> methods, ConstructorInfo? constructorInfo)
    {
        ClassName = className;
        Methods = methods;
        ConstructorInfo = constructorInfo;
        Namespace = namepsace;
    }
}
