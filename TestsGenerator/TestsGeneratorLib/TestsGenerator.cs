﻿using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestsGeneratorLib;

public class TestsGenerator
{
    public Task Generate(IList<string> files, string resDir, int maxInFiles, int maxProcFiles, int maxOutFiles)
    {
        DataflowLinkOptions linkOptions = new() { PropagateCompletion = true };

        ExecutionDataflowBlockOptions readingBlockOptions = new() { MaxDegreeOfParallelism = maxInFiles };
        ExecutionDataflowBlockOptions processingBlockOptions = new() { MaxDegreeOfParallelism = maxProcFiles };
        ExecutionDataflowBlockOptions outputBlockOptions = new() { MaxDegreeOfParallelism = maxOutFiles };

        TransformBlock<string, string> readingFilesBlock
            = new(async path => await File.ReadAllTextAsync(path), readingBlockOptions);

        TransformManyBlock<string, TestClassInfo> processingBlock
            = new(content => ProcessFile(content), processingBlockOptions);

        ActionBlock<TestClassInfo> outputBlock = new(
            info => File.WriteAllTextAsync(Path.Combine(resDir, info.ClassName + ".cs"), info.Content),
            outputBlockOptions
            );

        readingFilesBlock.LinkTo(processingBlock, linkOptions);
        processingBlock.LinkTo(outputBlock, linkOptions);

        foreach (var file in files)
        {
            readingFilesBlock.Post(file);
        }

        readingFilesBlock.Complete();

        return outputBlock.Completion;
    }

    private IList<TestClassInfo> ProcessFile(string content)
    {
        IList<ClassDeclarationInfo> infos = GetClassDeclarations(content);

        return infos.Select(i =>
        new TestClassInfo(i.ClassName, CreateTestClass(i.ClassName, i.MethodsNames)))
            .ToList();
    }

    private IList<ClassDeclarationInfo> GetClassDeclarations(string fileContent)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(fileContent);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot(); 

        ClassesCollector collector = new();

        collector.Visit(root);

        return collector.ClassesInfo;
    }

    private string CreateTestClass(string className, IList<string> methods)
    {
        AttributeSyntax methodAttr = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestMethod"), null);
        AttributeListSyntax methodAttributeList = SyntaxFactory.AttributeList();
        methodAttributeList = methodAttributeList.AddAttributes(methodAttr);
        SyntaxList<AttributeListSyntax> methodAttributeLists = SyntaxFactory.List<AttributeListSyntax>([methodAttributeList]);

        ExpressionSyntax expression = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.IdentifierName("Assert"),
            SyntaxFactory.Token(SyntaxKind.DotToken),
            SyntaxFactory.IdentifierName("Fail")
            );

        SyntaxToken stringLiteralToken = SyntaxFactory.Literal("autogenerated");
        ExpressionSyntax stringLiteralExp = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, stringLiteralToken);
        ArgumentSyntax arg = SyntaxFactory.Argument(stringLiteralExp);

        var args = SyntaxFactory.SeparatedList<ArgumentSyntax>();
        args = args.Add(arg);
        InvocationExpressionSyntax invocation = SyntaxFactory.InvocationExpression(
            expression,
            SyntaxFactory.ArgumentList(args)
            );

        List<MethodDeclarationSyntax> methodDeclarations = new();
        foreach (var method in methods)
        {
            var methodSyntax = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                method + "Test")
                .WithAttributeLists(methodAttributeLists)
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(invocation, SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
                );

            methodDeclarations.Add( methodSyntax );
        }

        var members = SyntaxFactory.List<MemberDeclarationSyntax>(methodDeclarations);

        AttributeSyntax attr = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("TestClass"), null);
        AttributeListSyntax attributeList = SyntaxFactory.AttributeList(
            SyntaxFactory.Token(SyntaxKind.OpenBracketToken),
            null,
            SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(attr),
            SyntaxFactory.Token(SyntaxKind.CloseBracketToken)
            );

        SyntaxList<AttributeListSyntax> attributeLists = SyntaxFactory.List<AttributeListSyntax>([attributeList]);

        SyntaxToken publicModifier = SyntaxFactory.Token(SyntaxKind.PublicKeyword);
        SyntaxTokenList modifiers = SyntaxFactory.TokenList([publicModifier]);

        SyntaxTriviaList beforeIdentifier = SyntaxFactory.TriviaList();
        SyntaxTriviaList afterIdentifier = SyntaxFactory.TriviaList();
        SyntaxToken identifier = SyntaxFactory.Identifier(
            beforeIdentifier,
            SyntaxKind.IdentifierToken,
            className + "Tests",
            className + "Tests",
            afterIdentifier
            );

        ClassDeclarationSyntax classDeclarationSyntax = SyntaxFactory.ClassDeclaration(
            attributeLists,
            modifiers,
            identifier,
            null,
            null,
            SyntaxFactory.List<TypeParameterConstraintClauseSyntax>(),
            members
            );

        var fileScopedNamespaceDecl = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName("Tests"));

        var compilationUnit = SyntaxFactory.CompilationUnit()
            .AddMembers(fileScopedNamespaceDecl
                .AddMembers(classDeclarationSyntax)
            );

        var code = compilationUnit.NormalizeWhitespace().ToFullString();
        return code.ToString();
    }

}
