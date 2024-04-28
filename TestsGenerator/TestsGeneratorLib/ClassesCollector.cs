using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLib;

public class ClassesCollector : CSharpSyntaxWalker {
    public IList<ClassDeclarationInfo> ClassesInfo
    { get; } = new List<ClassDeclarationInfo>();

    private string? _namespace = null;
    
    private bool IsInterfaceIdentifier(string identifier)
    {
        return identifier.StartsWith('I')
            && identifier.Length > 1
            && Char.IsUpper(identifier[1]);
    }

    private ConstructorInfo? CheckConstructor(ConstructorDeclarationSyntax constructor)
    {
        String constructorName = constructor
            .ChildTokens()
            .First(t => t.IsKind(SyntaxKind.IdentifierToken))
            .ToString();

        List<ParameterInfo> parameters;
        try
        {
            parameters = constructor.ChildNodes()
                .OfType<ParameterListSyntax>()
                .First()
                .ChildNodes()
                .OfType<ParameterSyntax>()
                .Select(n => 
                    new ParameterInfo(
                        n
                            .ChildNodes()
                            .OfType<IdentifierNameSyntax>()
                            .First(n => IsInterfaceIdentifier(n.ToString()))
                            .ToString(),
                        n.ChildTokens()
                            .First()
                            .ToString()
                    )
                )
                .ToList();
        }
        catch (InvalidOperationException e)
        {
            return null;
        }

        if (parameters.Count == 0)
        {
            return null;
        }

        return new ConstructorInfo(parameters);
    }

    public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
    {
        _namespace = node.Name.ToString();

        foreach (var childNode in node.ChildNodes())
        {
            Visit(childNode);
        }
    }

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        _namespace = node.Name.ToString();
        
        foreach (var childNode in node.ChildNodes())
        {
            Visit(childNode);
        }
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        SyntaxToken className = node
            .ChildTokens()
            .First(token => token.IsKind(SyntaxKind.IdentifierToken));

        var constructors = node
            .DescendantNodes()
            .OfType<ConstructorDeclarationSyntax>();

        ConstructorInfo? constructorInfo = null;

        foreach ( var constructor in constructors )
        {
            constructorInfo = CheckConstructor(constructor);
            if ( constructorInfo != null )
            {
                break;
            }
        }

        List<MethodDeclarationInfo> methods = node
            .ChildNodes()
            .Where(n => n.IsKind(SyntaxKind.MethodDeclaration)
                && n.DescendantTokens().Any(n => n.IsKind(SyntaxKind.PublicKeyword)))
            .Select(n => 
                new MethodDeclarationInfo( 
                    n
                        .ChildTokens()
                        .First(t => t.IsKind(SyntaxKind.IdentifierToken))
                        .ToString(),
                    n.DescendantNodes()
                        .Where(n =>
                            n.IsKind(SyntaxKind.Parameter))
                        .Select(n =>
                            new ParameterInfo(
                                n.ChildNodes().First().ChildTokens().First().ToString(),
                                n.ChildTokens().First().ToString()
                            )
                        )
                        .ToList(),
                    n.DescendantNodes().First().ToString()
                )
            ).ToList();

        ClassesInfo.Add(
            new ClassDeclarationInfo(
                _namespace!,
                className.ToString(),
                methods,
                constructorInfo
            )
        );
    }
}

