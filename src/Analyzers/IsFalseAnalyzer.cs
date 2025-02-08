using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IsFalse.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IsFalseAnalyzer : DiagnosticAnalyzer
{
    public const string IsFalseDiagnosticId = "IF001";
    public const string DoubleNegativeDiagnosticId = "IF002";

    private static readonly LocalizableString TitleIF001 = "Use IsFalse()";
    private static readonly LocalizableString MessageFormatIF001 = "Replace negation ('!') with IsFalse() for improved readability";

    private static readonly LocalizableString TitleIF002 = "Simplify double negative condition";
    private static readonly LocalizableString MessageFormatIF002 = "Remove redundant double negation for better readability";

    private static readonly DiagnosticDescriptor RuleIF001 = new(
        IsFalseDiagnosticId, TitleIF001, MessageFormatIF001, "Style", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor RuleIF002 = new(
        DoubleNegativeDiagnosticId, TitleIF002, MessageFormatIF002, "Style", DiagnosticSeverity.Warning, isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(RuleIF001, RuleIF002);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.LogicalNotExpression);
    }

    private void AnalyzeNode(SyntaxNodeAnalysisContext context)
    {
        var notExpression = (PrefixUnaryExpressionSyntax)context.Node;
        ExpressionSyntax operand = notExpression.Operand;
        var typeInfo = context.SemanticModel.GetTypeInfo(operand);

        if (typeInfo.ConvertedType?.SpecialType == SpecialType.System_Boolean)
        {
            // Case 1: Recommend using IsFalse() instead of !
            if (operand is not InvocationExpressionSyntax)
            {
                var diagnostic = Diagnostic.Create(RuleIF001, notExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
            // Case 2: Remove redundant !something.IsFalse()
            else if (operand is InvocationExpressionSyntax invocation &&
                     invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
                     memberAccess.Name.Identifier.Text == "IsFalse")
            {
                var diagnostic = Diagnostic.Create(RuleIF002, notExpression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
