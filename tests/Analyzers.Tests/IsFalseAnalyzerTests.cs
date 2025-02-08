using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<
    IsFalse.Analyzers.IsFalseAnalyzer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Microsoft.CodeAnalysis.CSharp.Testing;

namespace IsFalse.Analyzers.Tests.Analyzers;

public class IsFalseAnalyzerTests : TestBase
{
    [Fact]
    public async Task Should_ReportDiagnosticWhenNegatingBoolean_WithNotOperator()
    {
        var testCode = GetTestCode(TestCondition.UsingPresent);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.IsFalseDiagnosticId)
                             .WithLocation(10, 13)
                             .WithSeverity(DiagnosticSeverity.Warning)
                             .WithMessage("Instead of using '!' on a boolean, use IsFalse()");

        await new CSharpAnalyzerTest<IsFalseAnalyzer, DefaultVerifier>
        {
            TestCode = testCode,
            ExpectedDiagnostics = { expected },
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly }
            }
        }.RunAsync();
    }

    [Fact]
    public async Task Should_ReportDiagnosticWhenNegatingBoolean_WithNotOperator_AndWithIsFalseMethod()
    {
        var testCode = GetTestCode(TestCondition.DoubleNegatives);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.DoubleNegativeDiagnosticId)
                             .WithLocation(10, 13);

        var test = new CSharpAnalyzerTest<IsFalseAnalyzer, DefaultVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly }
            }
        };

        test.ExpectedDiagnostics.Add(expected);

        await test.RunAsync();
    }

    [Fact]
    public async Task ShouldNot_ReportDiagnosticWhenNegatingBoolean_WithIsFalseMethod()
    {
        var testCode = GetTestCode(TestCondition.DiagnosticNotRequired);

        await new CSharpAnalyzerTest<IsFalseAnalyzer, DefaultVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly }
            }
        }.RunAsync();
    }
}
