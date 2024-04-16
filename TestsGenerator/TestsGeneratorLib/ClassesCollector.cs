using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLib;

public class ClassesCollector : CSharpSyntaxWalker {
    public IList<ClassDeclarationInfo> ClassesInfo
    { get; } = new List<ClassDeclarationInfo>();

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        SyntaxToken className = node
            .ChildTokens()
            .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
            .First();

        var methods = node
            .ChildNodes()
            .Where(n => n.IsKind(SyntaxKind.MethodDeclaration)
                && n.DescendantTokens().Any(n => n.IsKind(SyntaxKind.PublicKeyword)))
            .Select(n=> n.ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.IdentifierToken)).First());

        ClassesInfo.Add(
            new ClassDeclarationInfo(className.ToString(),
            methods.Select(m => m.ToString()).ToList())
            );
    }
}

