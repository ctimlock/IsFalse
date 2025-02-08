namespace IsFalse.Analyzers.Tests;

public class TestBase
{
    private const string TEST_CODE_FOLDER = "./TestCode";

    public enum TestCondition
    {
        DiagnosticNotRequired,
        DoubleNegatives,
        UsingNotPresent,
        UsingPresent,
        GlobalUsingPresent,
    }

    public static string GetFixedCode(TestCondition testCondition)
    {
        return File.ReadAllText($"{TEST_CODE_FOLDER}/{testCondition}/FixedCode.cs");
    }

    public static string GetTestCode(TestCondition usingStatus)
    {
        return File.ReadAllText($"{TEST_CODE_FOLDER}/{usingStatus}/TestCode.cs");
    }

    public static string GetGlobalUsings()
    {
        return File.ReadAllText($"{TEST_CODE_FOLDER}/GlobalUsings/Usings.cs");
    }
}