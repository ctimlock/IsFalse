using System.Composition;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using IsFalse.Analyzers;
using System;

namespace IsFalse.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IsFalseCodeFixProvider)), Shared]
public class IsFalseCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(IsFalseAnalyzer.IsFalseDiagnosticId, IsFalseAnalyzer.DoubleNegativeDiagnosticId);

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root?.FindNode(diagnosticSpan) is not PrefixUnaryExpressionSyntax syntaxNode)
            return;

        string equivalenceKey;
        Func<CancellationToken, Task<Document>> fixFunction;

        if (diagnostic.Id == IsFalseAnalyzer.IsFalseDiagnosticId)
        {
            equivalenceKey = "UseIsFalse";
            fixFunction = c => ApplyIsFalseFixAsync(context.Document, syntaxNode, c);
        }
        else if (diagnostic.Id == IsFalseAnalyzer.DoubleNegativeDiagnosticId)
        {
            equivalenceKey = "RemoveRedundantNegation";
            fixFunction = c => ApplyRemoveRedundantNegationFixAsync(context.Document, syntaxNode, c);
        }
        else
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: diagnostic.Descriptor.Title.ToString(),
                createChangedDocument: fixFunction,
                equivalenceKey: equivalenceKey),
            diagnostic);
    }

    private static async Task<Document> ApplyIsFalseFixAsync(Document document, PrefixUnaryExpressionSyntax syntaxNode, CancellationToken cancellationToken)
    {
        var operand = syntaxNode.Operand is ParenthesizedExpressionSyntax parenthesized
            ? parenthesized.Expression
            : syntaxNode.Operand;

        var newExpression = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                operand,
                SyntaxFactory.IdentifierName("IsFalse")))
            .WithAdditionalAnnotations(Formatter.Annotation);

        var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
        editor.ReplaceNode(syntaxNode, newExpression);
        return await EnsureUsingDirectiveAsync(editor.GetChangedDocument(), cancellationToken);
    }

    private static async Task<Document> ApplyRemoveRedundantNegationFixAsync(Document document, PrefixUnaryExpressionSyntax syntaxNode, CancellationToken cancellationToken)
    {
        if (syntaxNode.Operand is InvocationExpressionSyntax invocation &&
            invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var newExpression = memberAccess.Expression.WithAdditionalAnnotations(Formatter.Annotation);
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            editor.ReplaceNode(syntaxNode, newExpression);
            return editor.GetChangedDocument();
        }

        return document;
    }

    private static async Task<Document> EnsureUsingDirectiveAsync(Document document, CancellationToken cancellationToken)
    {
        var compilation = await document.Project.GetCompilationAsync(cancellationToken).ConfigureAwait(false);
        if (compilation != null)
        {
            bool hasGlobalUsing = compilation.SyntaxTrees
                .Select(tree => tree.GetRoot(cancellationToken))
                .OfType<CompilationUnitSyntax>()
                .Any(root => root.Usings.Any(u => u.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword) &&
                                                   u.Name.ToString() == "IsFalse"));
            if (hasGlobalUsing)
                return document;
        }

        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is not CompilationUnitSyntax compilationUnit)
            return document;

        if (compilationUnit.Usings.Any(u => u.Name.ToString() == "IsFalse"))
            return document;

        var newUsing = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("IsFalse"))
            .WithAdditionalAnnotations(Formatter.Annotation);
        int insertIndex = compilationUnit.Usings
            .TakeWhile(u => string.Compare(u.Name.ToString(), "IsFalse", StringComparison.Ordinal) <= 0)
            .Count();
        var newCompilationUnit = compilationUnit.WithUsings(compilationUnit.Usings.Insert(insertIndex, newUsing));
        return document.WithSyntaxRoot(newCompilationUnit);
    }
}
