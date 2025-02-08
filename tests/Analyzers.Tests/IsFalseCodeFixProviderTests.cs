using Microsoft.CodeAnalysis.Testing;

using Verify = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    IsFalse.Analyzers.IsFalseAnalyzer,
    IsFalse.CodeFixProviders.IsFalseCodeFixProvider,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using Microsoft.CodeAnalysis.CSharp.Testing;
using IsFalse.CodeFixProviders;

namespace IsFalse.Analyzers.Tests.CodeFixProviders;

public class IsFalseCodeFixProviderTests : TestBase
{
    [Fact]
    public async Task Should_AddUsingAndMethodWhenUsingNotPresent()
    {
        var testCode = GetTestCode(TestCondition.UsingNotPresent);

        var fixedCode = GetFixedCode(TestCondition.UsingNotPresent);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.IsFalseDiagnosticId)
                             .WithSpan(8, 13, 8, 18);

        var test = new CSharpCodeFixTest<IsFalseAnalyzer, IsFalseCodeFixProvider, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestCode = testCode,
            FixedCode = fixedCode,
            ExpectedDiagnostics = { expected },
            FixedState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly }
            }
        };

        test.TestState.AdditionalReferences.Add(typeof(BooleanExtensions).Assembly);

        await test.RunAsync();
    }

    [Fact]
    public async Task Should_NotAddUsingWhenUsingPresent()
    {
        var testCode = GetTestCode(TestCondition.UsingPresent);

        var fixedCode = GetFixedCode(TestCondition.UsingPresent);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.IsFalseDiagnosticId)
                             .WithSpan(10, 13, 10, 18);

        var test = new CSharpCodeFixTest<IsFalseAnalyzer, IsFalseCodeFixProvider, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestCode = testCode,
            FixedCode = fixedCode,
            ExpectedDiagnostics = { expected },
            FixedState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly }
            }
        };

        test.TestState.AdditionalReferences.Add(typeof(BooleanExtensions).Assembly);

        await test.RunAsync();
    }

    [Fact]
    public async Task Should_NotAddUsingWhenGlobalUsingPresent()
    {
        var testCode = GetTestCode(TestCondition.GlobalUsingPresent);

        var fixedCode = GetFixedCode(TestCondition.GlobalUsingPresent);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.IsFalseDiagnosticId)
                             .WithSpan(8, 13, 8, 18);

        var test = new CSharpCodeFixTest<IsFalseAnalyzer, IsFalseCodeFixProvider, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestCode = testCode,
            FixedCode = fixedCode,
            ExpectedDiagnostics = { expected },
            FixedState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly },
                Sources = { GetGlobalUsings() }
            }
        };

        test.TestState.Sources.Add(GetGlobalUsings());
        test.TestState.AdditionalReferences.Add(typeof(BooleanExtensions).Assembly);

        await test.RunAsync();
    }

    [Fact]
    public async Task Should_RemoveDoubleNegative()
    {
        var testCode = GetTestCode(TestCondition.DoubleNegatives);

        var fixedCode = GetFixedCode(TestCondition.DoubleNegatives);

        var expected = Verify.Diagnostic(IsFalseAnalyzer.DoubleNegativeDiagnosticId)
                             .WithSpan(10, 13, 10, 28);

        var test = new CSharpCodeFixTest<IsFalseAnalyzer, IsFalseCodeFixProvider, DefaultVerifier>
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows,
            TestCode = testCode,
            FixedCode = fixedCode,
            ExpectedDiagnostics = { expected },
            FixedState =
            {
                AdditionalReferences = { typeof(BooleanExtensions).Assembly },
            }
        };

        test.TestState.AdditionalReferences.Add(typeof(BooleanExtensions).Assembly);

        await test.RunAsync();
    }
}
