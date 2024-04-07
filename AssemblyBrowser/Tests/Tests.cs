using System.Reflection;
using System.Runtime.CompilerServices;
using AssemblyScannerLib;
using FluentAssertions;
using Newtonsoft.Json.Serialization;
using TypeInfo = AssemblyScannerLib.TypeInfo;

namespace Tests;

[TestClass]
public class Tests
{
    public Dictionary<string, NamespaceInfo> LoadNamespaces()
    {
        string assemblyPath = "C:\\Users\\tisha\\BSUIR\\sem6\\Mpp\\Labs\\AssemblyBrowser\\ForTests\\bin\\Debug\\net8.0\\ForTests.dll";

        AssemblyScanner assemblyScanner = new();

        Dictionary<string, NamespaceInfo> assemblyInfo = assemblyScanner.Scan(assemblyPath);

        return assemblyInfo;
    }

    [TestMethod]
    public void TestFieldPropetyMethodScanning()
    {
        Dictionary<string, NamespaceInfo> namespaces = LoadNamespaces();

        NamespaceInfo namespaceInfo = namespaces["ForTests"];

        TypeInfo typeInfo = namespaceInfo.Types["TestFieldPropertyMethodScanning"];

        MethodData methodData = typeInfo.Methods.Where(methodData => methodData.Name.Equals("C")).First();

        MethodData expectedMethodData = new MethodData();

        expectedMethodData.ReturnType = typeof(string);
        expectedMethodData.AccessModifier = MethodAttributes.Public;
        expectedMethodData.Params = new();
        expectedMethodData.IsStatic = false;
        expectedMethodData.IsVirtual = false;
        expectedMethodData.IsSealed = false;
        expectedMethodData.IsAbstract = false;
        expectedMethodData.Name = "C";

        methodData.Should().Be(expectedMethodData);
    }

    [TestMethod]
    public void TestAccessModifiersScanning()
    {
        Dictionary<string, NamespaceInfo> namespaces = LoadNamespaces();

        NamespaceInfo namespaceInfo = namespaces["ForTests"];

        TypeInfo typeInfo = namespaceInfo.Types["TestAccessModifiersScanning"];

        Dictionary<string, MethodAttributes> methodToAccessModifier = new()
        {
            { "a", MethodAttributes.Private },
            { "b", MethodAttributes.FamANDAssem },
            { "c", MethodAttributes.Assembly},
            { "d", MethodAttributes.Family },
            { "e", MethodAttributes.FamORAssem },
            { "f", MethodAttributes.Public }
        };

        foreach (MethodData methodData in typeInfo.Methods)
        {
            if (methodToAccessModifier.ContainsKey(methodData.Name))
            {
                MethodAttributes accessModifier = methodToAccessModifier[methodData.Name];
                methodData.AccessModifier.Should().Be(accessModifier);
            }
        }
    }

    [TestMethod]
    public void TestOptionalModifiersScanning()
    {
        Dictionary<string, NamespaceInfo> namespaces = LoadNamespaces();

        NamespaceInfo namespaceInfo = namespaces["ForTests"];

        TypeInfo typeInfo = namespaceInfo.Types["TestOptionalModifiersScanning"];

        List<MethodData> methods = typeInfo.Methods;
        MethodData staticMethod = (from methodData in methods
                             where methodData.Name.Equals("StaticMethod")
                             select methodData).First();

        staticMethod.IsStatic.Should().Be(true);
        staticMethod.IsVirtual.Should().Be(false);
        staticMethod.IsSealed.Should().Be(false);
        staticMethod.IsAbstract.Should().Be(false);


        MethodData virtualMethod = (from methodData in methods
                             where methodData.Name.Equals("VirtualMethod")
                             select methodData).First();

        virtualMethod.IsStatic.Should().Be(false);
        virtualMethod.IsVirtual.Should().Be(true);
        virtualMethod.IsSealed.Should().Be(false);
        virtualMethod.IsAbstract.Should().Be(false);

        MethodData sealedMethod = (from methodData in methods
                             where methodData.Name.Equals("ToString")
                             select methodData).First();

        sealedMethod.IsStatic.Should().Be(false);
        sealedMethod.IsVirtual.Should().Be(true);
        sealedMethod.IsSealed.Should().Be(true);
        sealedMethod.IsAbstract.Should().Be(false);

        MethodData abstractMethod = (from methodData in methods
                             where methodData.Name.Equals("AbstractMethod")
                             select methodData).First();

        abstractMethod.IsStatic.Should().Be(false);
        abstractMethod.IsVirtual.Should().Be(true);
        abstractMethod.IsSealed.Should().Be(false);
        abstractMethod.IsAbstract.Should().Be(true);
    }

    [TestMethod]
    public void TestExtensionMethodScanning()
    {
        Dictionary<string, NamespaceInfo> namespaces = LoadNamespaces();

        NamespaceInfo namespaceInfo = namespaces["ForTests"];

        TypeInfo extendedClass = namespaceInfo.Types["ExtendedClass"];
        (from methodData in extendedClass.ExtensionMethods
         where methodData.Name.Equals("ExtensionMethod")
         select methodData).Count().Should().Be(1);

        TypeInfo classWithExtensionMethod = namespaceInfo.Types["ClassWithExtensionMethod"];
        (from methodData in classWithExtensionMethod.ExtensionMethods
         where methodData.Name.Equals("ExtensionMethod")
         select methodData).Count().Should().Be(0);
    }

    [TestMethod]
    public void TestNestedNamespaceScanning()
    {
        Dictionary<string, NamespaceInfo> namespaces = LoadNamespaces();

        NamespaceInfo namespaceInfo = namespaces["ForTests"];

        TypeInfo typeInfo = namespaceInfo
            .NestedNamespaces["NestedNamespace"]
            .Types["ClassIntoNestedNamespace"];

        typeInfo.Should().Be(typeInfo);
    }
}

