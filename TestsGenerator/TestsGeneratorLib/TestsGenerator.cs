using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TestsGeneratorLib;

public class TestsGenerator
{
    private static readonly string VoidTypeName = "void";
    
    public Task Generate(IList<string> files, string resDir, int maxInFiles, int maxProcFiles, int maxOutFiles)
    {
        DataflowLinkOptions linkOptions = new() { PropagateCompletion = true };

        ExecutionDataflowBlockOptions readingBlockOptions = new() { MaxDegreeOfParallelism = maxInFiles };
        ExecutionDataflowBlockOptions processingBlockOptions = new() { MaxDegreeOfParallelism = maxProcFiles };
        ExecutionDataflowBlockOptions outputBlockOptions = new() { MaxDegreeOfParallelism = maxOutFiles };

        TransformBlock<string, string> readingFilesBlock
            = new(async path => await File.ReadAllTextAsync(path), readingBlockOptions);

        TransformManyBlock<string, TestClassInfo> processingBlock
            = new(ProcessFile, processingBlockOptions);

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
        new TestClassInfo(i.ClassName, CreateTestClass(i)))
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

    public StatementSyntax CreateLocalVariable(ParameterInfo parameterInfo)
    {
        return 
                LocalDeclarationStatement(
                    VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(
                                    Whitespace("            ")),
                                parameterInfo.Type,
                                TriviaList(
                                    Space))))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                Identifier(
                                    TriviaList(),
                                    parameterInfo.Name,
                                    TriviaList(
                                        Space)))
                            .WithInitializer(
                                EqualsValueClause(
                                    DefaultExpression(
                                        IdentifierName(parameterInfo.Type)))
                                .WithEqualsToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.EqualsToken,
                                        TriviaList(
                                            Space)))))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)));
    }

    public ArgumentListSyntax CreateMethodArgumentList(MethodDeclarationInfo methodInfo)
    {
        return
            ArgumentList(
                SeparatedList<ArgumentSyntax>(
                    methodInfo
                        .Parameters
                        .SelectMany<ParameterInfo, SyntaxNodeOrToken>(i =>
                        [
                            Argument(
                                IdentifierName(i.Name)),
                            Token(
                                TriviaList(),
                                SyntaxKind.CommaToken,
                                TriviaList(
                                    Space))
                        ])
                        .Take(..^1)
                        .ToArray()
                )
            );
    }
    
    public IEnumerable<StatementSyntax> CreateArrangeSection(IEnumerable<ParameterInfo> parameters)
    {
        List<StatementSyntax> nodes = parameters
            .Select(CreateLocalVariable)
            .ToList();

        return nodes;
    }

    public List<StatementSyntax> CreateVoidActSection(
        MethodDeclarationInfo methodInfo,
        string classFieldName
        )
    {
        ArgumentListSyntax methodArgs = CreateMethodArgumentList(methodInfo);
        
        return [
            ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(
                                Identifier(
                                        TriviaList(
                                            LineFeed,
                                            Whitespace("            ")
                                        ),
                                        classFieldName,
                                        TriviaList()
                                )
                            ),
                            IdentifierName(methodInfo.Name)))
                    .WithArgumentList(
                        methodArgs
                    ))
            .WithSemicolonToken(
                Token(
                    TriviaList(),
                    SyntaxKind.SemicolonToken,
                    TriviaList(
                        LineFeed)))
        ];
    }
    
    public List<StatementSyntax> CreateNotVoidActSection(
        MethodDeclarationInfo methodInfo,
        string classFieldName
        )
    {
        ArgumentListSyntax methodArgs = CreateMethodArgumentList(methodInfo);
        
        return [ 
            LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(
                                        LineFeed,
                                        Whitespace("            ")
                                    ),
                                    methodInfo.ReturnType,
                                    TriviaList(
                                        Space))))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                        Identifier(
                                            TriviaList(),
                                            "actual",
                                            TriviaList(
                                                Space)))
                                    .WithInitializer(
                                        EqualsValueClause(
                                                InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName(classFieldName),
                                                            IdentifierName(methodInfo.Name)))
                                                    .WithArgumentList(
                                                        methodArgs
                                                    ))
                                            .WithEqualsToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.EqualsToken,
                                                    TriviaList(
                                                        Space)))))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed,
                            LineFeed)))
        ];
    }

    public List<StatementSyntax> CreateNotVoidAssertSection(MethodDeclarationInfo methodInfo)
    {
        List<StatementSyntax> nodes =
        [
            LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(
                                            Whitespace("            ")
                                        ),
                                    methodInfo.ReturnType,
                                    TriviaList(
                                        Space))))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                        Identifier(
                                            TriviaList(),
                                            "expected",
                                            TriviaList(
                                                Space)))
                                    .WithInitializer(
                                        EqualsValueClause(
                                                DefaultExpression(
                                                    IdentifierName(methodInfo.ReturnType)))
                                            .WithEqualsToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.EqualsToken,
                                                    TriviaList(
                                                        Space)))))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed))),

            ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(
                                    Identifier(
                                        TriviaList(Whitespace("            ")),
                                        "Assert",
                                        TriviaList()
                                    )
                                ),
                                IdentifierName("That")))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList<ArgumentSyntax>(
                                    new SyntaxNodeOrToken[]
                                    {
                                        Argument(
                                            IdentifierName("actual")),
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CommaToken,
                                            TriviaList(
                                                Space)),
                                        Argument(
                                            InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        IdentifierName("Is"),
                                                        IdentifierName("EqualTo")))
                                                .WithArgumentList(
                                                    ArgumentList(
                                                        SingletonSeparatedList<ArgumentSyntax>(
                                                            Argument(
                                                                IdentifierName("expected"))))))
                                    }))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed))),

            ExpressionStatement(
                    InvocationExpression(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName(
                                    Identifier(
                                        TriviaList(Whitespace("            ")),
                                        "Assert",
                                        TriviaList()
                                    )
                                ),
                                IdentifierName("Fail")))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal("autogenerated")))))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
        ];

        return nodes;
    }

    public List<StatementSyntax> CreateVoidAssertSection()
    {
        return [
            ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName(
                                Identifier(
                                    TriviaList(
                                        LineFeed,
                                        Whitespace("            ")
                                    ),
                                    "Assert",
                                    TriviaList()
                                )
                            ),
                            IdentifierName("Fail")))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("autogenerated")))))))
            .WithSemicolonToken(
                Token(
                    TriviaList(),
                    SyntaxKind.SemicolonToken,
                    TriviaList(
                        LineFeed)))
        ];
    }
    
    public IEnumerable<FieldDeclarationSyntax> CreateFieldDeclarations(List<ParameterInfo> parameters, FieldInfo classFieldInfo)
    {
        List<FieldDeclarationSyntax> res = parameters.Select( param =>
            FieldDeclaration(
                VariableDeclaration(
                    GenericName(
                        Identifier("Mock"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(param.Type)))
                        .WithGreaterThanToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.GreaterThanToken,
                                TriviaList(
                                    Space)))))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(param.Name)))))
            .WithModifiers(
                TokenList(
                    Token(
                        TriviaList(
                            Whitespace("        ")),
                        SyntaxKind.PrivateKeyword,
                        TriviaList(
                            Space))))
            .WithSemicolonToken(
                Token(
                    TriviaList(),
                    SyntaxKind.SemicolonToken,
                    TriviaList(
                        LineFeed)))        
        ).ToList();

        res.Add(
            FieldDeclaration(
                    VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(),
                                    classFieldInfo.Type,
                                    TriviaList(
                                        Space))))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier(classFieldInfo.Name)))))
                .WithModifiers(
                    TokenList(
                        Token(
                            TriviaList(
                                Whitespace("        ")),
                            SyntaxKind.PrivateKeyword,
                            TriviaList(
                                Space))))
                .WithSemicolonToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.SemicolonToken,
                        TriviaList(
                            LineFeed)))
        );

        return res;
    }

    public MethodDeclarationSyntax CreateInitializeMethod(List<FieldInfo> fields, FieldInfo classFieldInfo)
    {
        List<StatementSyntax> methodStatements = new();
        methodStatements.AddRange(
                    fields.Select(field => 
                        ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                IdentifierName(
                                    Identifier(
                                        TriviaList(
                                            Whitespace("            ")),
                                        field.Name,
                                        TriviaList(
                                            Space))),
                                ObjectCreationExpression(
                                    GenericName(
                                        Identifier("Mock"))
                                    .WithTypeArgumentList(
                                        TypeArgumentList(
                                            SingletonSeparatedList<TypeSyntax>(
                                                IdentifierName(field.Type)))))
                                .WithNewKeyword(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.NewKeyword,
                                        TriviaList(
                                            Space)))
                                .WithArgumentList(
                                    ArgumentList()))
                            .WithOperatorToken(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.EqualsToken,
                                    TriviaList(
                                        Space))))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed)))
                    )
                );
        
        methodStatements.Add(
                ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(
                        Identifier(
                            TriviaList(
                                Whitespace("            ")),
                            classFieldInfo.Name,
                            TriviaList(
                                Space))),
                    ObjectCreationExpression(
                        IdentifierName(classFieldInfo.Type))
                    .WithNewKeyword(
                        Token(
                            TriviaList(),
                            SyntaxKind.NewKeyword,
                            TriviaList(
                                Space)))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                fields.Select(f =>
                                Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(f.Name),
                                        IdentifierName("Object"))))))))
                .WithOperatorToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.EqualsToken,
                        TriviaList(
                            Space))))
            .WithSemicolonToken(
                Token(
                    TriviaList(),
                    SyntaxKind.SemicolonToken,
                    TriviaList(
                        LineFeed)))
        ); 
    
    return
            MethodDeclaration(
                PredefinedType(
                    Token(
                        TriviaList(),
                        SyntaxKind.VoidKeyword,
                        TriviaList(
                            Space))),
                Identifier("Initialization"))
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestInitialization"))))
                    .WithOpenBracketToken(
                        Token(
                            TriviaList(
                                new []{
                                    LineFeed,
                                    Whitespace("        ")}),
                            SyntaxKind.OpenBracketToken,
                            TriviaList()))
                    .WithCloseBracketToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBracketToken,
                            TriviaList(
                                LineFeed)))))
            .WithModifiers(
                TokenList(
                    Token(
                        TriviaList(
                            Whitespace("        ")),
                        SyntaxKind.PublicKeyword,
                        TriviaList(
                            Space))))
            .WithParameterList(
                ParameterList()
                .WithCloseParenToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.CloseParenToken,
                        TriviaList(
                            LineFeed))))
            .WithBody(
                Block(
                        methodStatements
                )
                .WithOpenBraceToken(
                    Token(
                        TriviaList(
                            Whitespace("        ")),
                        SyntaxKind.OpenBraceToken,
                        TriviaList(
                            LineFeed)))
                .WithCloseBraceToken(
                    Token(
                        TriviaList(
                            Whitespace("        ")),
                        SyntaxKind.CloseBraceToken,
                        TriviaList())));
    }

    public MethodDeclarationSyntax CreateTestMethod(MethodDeclarationInfo methodInfo, FieldInfo classField)
    {
        List<StatementSyntax> methodBody = new();
        methodBody.AddRange(
            CreateArrangeSection(methodInfo.Parameters)
        );

        if (!methodInfo.ReturnType.Equals(VoidTypeName))
        {
            methodBody.AddRange(
                CreateNotVoidActSection(methodInfo, classField.Name)
            );
            methodBody.AddRange(
                CreateNotVoidAssertSection(methodInfo)
            );
        }
        else
        {
            methodBody.AddRange(
                CreateVoidActSection(methodInfo, classField.Name)
            );
            methodBody.AddRange(
                CreateVoidAssertSection()
            );
        }
        
        return 
                MethodDeclaration(
                    PredefinedType(
                        Token(
                            TriviaList(),
                            SyntaxKind.VoidKeyword,
                            TriviaList(
                                Space))),
                    Identifier(methodInfo.Name))
                .WithAttributeLists(
                    SingletonList<AttributeListSyntax>(
                        AttributeList(
                            SingletonSeparatedList<AttributeSyntax>(
                                Attribute(
                                    IdentifierName("TestMethod"))))
                        .WithOpenBracketToken(
                            Token(
                                TriviaList(
                                    new []{
                                        Whitespace("        "),
                                        LineFeed,
                                        Whitespace("        "),
                                        LineFeed,
                                        Whitespace("        ")}),
                                SyntaxKind.OpenBracketToken,
                                TriviaList()))
                        .WithCloseBracketToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.CloseBracketToken,
                                TriviaList(
                                    LineFeed)))))
                .WithModifiers(
                    TokenList(
                        Token(
                            TriviaList(
                                Whitespace("        ")),
                            SyntaxKind.PublicKeyword,
                            TriviaList(
                                Space))))
                .WithParameterList(
                    ParameterList()
                    .WithCloseParenToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseParenToken,
                            TriviaList(
                                LineFeed))))
                .WithBody(
                    Block(
                        methodBody
                    )
                    .WithOpenBraceToken(
                        Token(
                            TriviaList(
                                Whitespace("        ")),
                            SyntaxKind.OpenBraceToken,
                            TriviaList(
                                LineFeed)))
                    .WithCloseBraceToken(
                        Token(
                            TriviaList(
                                Whitespace("        ")),
                            SyntaxKind.CloseBraceToken,
                            TriviaList(
                                LineFeed))));
    }
    
    public string CreateTestClass(ClassDeclarationInfo classInfo)
    {
        string classFieldName = "_"
                                + classInfo.ClassName[..1].ToLower()
                                + classInfo.ClassName[1..];

        FieldInfo classFieldInfo = new FieldInfo(classInfo.ClassName, classFieldName);
        
        List<FieldInfo> classFields = classInfo
            .ConstructorInfo!.Value
            .Params
            .Select(p => new FieldInfo(p.Type, p.Name))
            .ToList();

        List<MemberDeclarationSyntax> classMembers = new();
        classMembers.AddRange(
            CreateFieldDeclarations(
                        classInfo.ConstructorInfo!.Value.Params,
                        classFieldInfo
                    ).ToList()
        );
            
        classMembers.Add(
            CreateInitializeMethod(classFields, classFieldInfo)
        );
        
        classMembers.AddRange(
            classInfo.Methods
            .Select(i =>
                CreateTestMethod(
                    i,
                    new FieldInfo(classInfo.ClassName, classFieldName)
                )
            )
        );
            
        return CompilationUnit()
            .WithUsings(
                List<UsingDirectiveSyntax>(
                    new UsingDirectiveSyntax[]{
                        UsingDirective(
                            IdentifierName("System"))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed))),
                        UsingDirective(
                            QualifiedName(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("Collections")),
                                IdentifierName("Generic")))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed))),
                        UsingDirective(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("Linq")))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed))),
                        UsingDirective(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("Text")))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed))),
                        UsingDirective(
                            IdentifierName("Moq"))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed))),
                        UsingDirective(
                            IdentifierName(classInfo.Namespace))
                        .WithUsingKeyword(
                            Token(
                                TriviaList(),
                                SyntaxKind.UsingKeyword,
                                TriviaList(
                                    Space)))
                        .WithSemicolonToken(
                            Token(
                                TriviaList(),
                                SyntaxKind.SemicolonToken,
                                TriviaList(
                                    LineFeed)))}))
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    NamespaceDeclaration(
                        QualifiedName(
                            IdentifierName(classInfo.Namespace),
                            IdentifierName(
                                Identifier(
                                    TriviaList(),
                                    "Tests",
                                    TriviaList(
                                        LineFeed)))))
                    .WithNamespaceKeyword(
                        Token(
                            TriviaList(
                                LineFeed),
                            SyntaxKind.NamespaceKeyword,
                            TriviaList(
                                Space)))
                    .WithOpenBraceToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.OpenBraceToken,
                            TriviaList(
                                LineFeed)))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            ClassDeclaration(
                                Identifier(
                                    TriviaList(),
                                    classInfo.ClassName,
                                    TriviaList(
                                        LineFeed)))
                            .WithAttributeLists(
                                SingletonList<AttributeListSyntax>(
                                    AttributeList(
                                        SingletonSeparatedList<AttributeSyntax>(
                                            Attribute(
                                                IdentifierName("TestClass"))))
                                    .WithOpenBracketToken(
                                        Token(
                                            TriviaList(
                                                Whitespace("    ")),
                                            SyntaxKind.OpenBracketToken,
                                            TriviaList()))
                                    .WithCloseBracketToken(
                                        Token(
                                            TriviaList(),
                                            SyntaxKind.CloseBracketToken,
                                            TriviaList(
                                                LineFeed)))))
                            .WithModifiers(
                                TokenList(
                                    Token(
                                        TriviaList(
                                            Whitespace("    ")),
                                        SyntaxKind.PublicKeyword,
                                        TriviaList(
                                            Space))))
                            .WithKeyword(
                                Token(
                                    TriviaList(),
                                    SyntaxKind.ClassKeyword,
                                    TriviaList(
                                        Space)))
                            .WithOpenBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("    ")),
                                    SyntaxKind.OpenBraceToken,
                                    TriviaList(
                                        LineFeed)))
                            .WithMembers(
                                List<MemberDeclarationSyntax>(
                                    classMembers
                                )
                            )
                            .WithCloseBraceToken(
                                Token(
                                    TriviaList(
                                        Whitespace("    ")),
                                    SyntaxKind.CloseBraceToken,
                                    TriviaList(
                                        LineFeed)))))
                    .WithCloseBraceToken(
                        Token(
                            TriviaList(),
                            SyntaxKind.CloseBraceToken,
                            TriviaList(
                                LineFeed))))).ToString();
    }
}
